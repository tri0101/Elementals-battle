using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CanvasReward : MonoBehaviour
{
    public static UI_CanvasReward Instance;

    [Header("UI")]
    [SerializeField] private Transform contentItem;
    [SerializeField] private Button buttonOk;

    [Header("Prefab")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPrefabShard;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject expItemPrefab;

    [SerializeField] private  List<GameObject> spawnedItems = new List<GameObject>();
    [SerializeField] private  Dictionary<int, UI_DropRewardItem> itemUIsById = new Dictionary<int, UI_DropRewardItem>();

    private Coroutine showCoroutine;
    private bool isShowing = false;

    #region INIT

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        buttonOk.onClick.AddListener(() => gameObject.SetActive(false));

        gameObject.SetActive(false);
    }
    
    #endregion

    #region CLEAR

    public void ClearOldItems()
    {
        foreach (Transform child in contentItem)
            Destroy(child.gameObject);

        spawnedItems.Clear();
        itemUIsById.Clear();

        if (showCoroutine != null)
            StopCoroutine(showCoroutine);

        isShowing = false;
    }

    #endregion

    #region SETUP (Item / Shard)

    public void SetUp(ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0)
            return;

        CreateOrAccumulateItem(itemData, amount);

        // Add to inventory (Item / Shard)
        PlayerInventory.Instance.AddItem(itemData.id, amount);
    }

    private void CreateOrAccumulateItem(ItemData data, int amount)
    {
        // If same item id already exists in UI -> accumulate instead of instantiate
        if (itemUIsById.TryGetValue(data.id, out var existingUi) && existingUi != null)
        {
            existingUi.AddAmount(amount);
            return;
        }

        GameObject prefab =
            data.id == 0
                ? (expItemPrefab ?? itemPrefab)
                : data.type == ItemType.HeroShard
                    ? (itemPrefabShard ?? itemPrefab)
                    : itemPrefab;

        var go = Instantiate(prefab, contentItem);

        var ui = go.GetComponent<UI_DropRewardItem>();
        if (ui != null)
        {
            ui.Setup(data, amount);
            itemUIsById[data.id] = ui;
        }

        go.SetActive(false);
        spawnedItems.Add(go);
    }

    #endregion

    #region SETUP (Hero)

    public void SetUp(HeroInfo heroInfo)
    {
        if (heroInfo == null)
            return;

        CreateHero(heroInfo);
        // Inventory add hero should be handled by caller (because it can become shard depending on owned state)
    }

    private void CreateHero(HeroInfo heroInfo)
    {
        if (heroPrefab == null)
        {
            Debug.LogError("UI_CanvasReward: heroPrefab is null.");
            return;
        }

        var go = Instantiate(heroPrefab, contentItem);

        var ui = go.GetComponent<UI_GachaResult>();
        if (ui != null)
            ui.SetUp(heroInfo);
        else
            Debug.LogError("UI_CanvasReward: heroPrefab missing UI_GachaResult component.");

        go.SetActive(false);
        spawnedItems.Add(go);
    }

    #endregion

    #region SETUP (BannerTokenExchangeData)

    public void SetUp(BannerTokenExchangeData data)
    {
        var result = PlayerInventory.Instance.AddHero(data.heroId);

        if (result.type == GachaResultType.Hero)
        {
            HeroInfo heroInfo = DatabaseManager.Instance.HeroDatabase.GetHero(data.heroId);
            SetUp(heroInfo);
        }
        else
        {
            int shardItemId = data.heroId + 1000;
            int shardAmount = 10;

            ItemData shardItem = DatabaseManager.Instance.ItemDatabase.GetItem(shardItemId);
            SetUp(shardItem, shardAmount);
        }
    }

    #endregion

    #region SHOW ANIMATION

    public void ShowReward()
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);

        showCoroutine = StartCoroutine(ShowItemsSequentially());
    }

    private IEnumerator ShowItemsSequentially()
    {
        isShowing = true;

        foreach (var item in spawnedItems)
        {
            item.SetActive(true);

            yield return StartCoroutine(PlayAppearEffect(item));

            yield return new WaitForSeconds(0.1f);
        }

        isShowing = false;
    }

    public void ShowAllImmediately()
    {
        if (!isShowing) return;

        if (showCoroutine != null)
            StopCoroutine(showCoroutine);

        foreach (var item in spawnedItems)
            item.SetActive(true);

        isShowing = false;
    }

    #endregion

    #region EFFECT

    private IEnumerator PlayAppearEffect(GameObject item)
    {
        RectTransform rect = item.GetComponent<RectTransform>();

        rect.localScale = Vector3.one * 0.8f;
        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(0.8f, 1f, t / duration);
            rect.localScale = Vector3.one * scale;
            yield return null;
        }

        rect.localScale = Vector3.one;

        Image flash = CreateFlashOverlay(item.transform);

        float flashTime = 0.25f;
        float elapsed = 0f;

        while (elapsed < flashTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.8f, 0f, elapsed / flashTime);
            flash.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        Destroy(flash.gameObject);
    }

    private Image CreateFlashOverlay(Transform parent)
    {
        GameObject flashObj = new GameObject("Flash");
        flashObj.transform.SetParent(parent);
        flashObj.transform.SetAsLastSibling();

        RectTransform rect = flashObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = flashObj.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 1.5f);

        return img;
    }

    #endregion

    #region INPUT

    private void Update()
    {
        if (isShowing && Input.GetMouseButtonDown(0))
        {
            ShowAllImmediately();
        }
    }

    #endregion
}