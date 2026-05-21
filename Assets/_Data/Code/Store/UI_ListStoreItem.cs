using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ListStoreItem : MonoBehaviour
{
    [SerializeField] private GameObject storeItemPrefabs;
    [SerializeField] private Transform contentItems;
    [SerializeField] private Button buttonRefresh;
    [SerializeField] private Button buttonOk;
    [SerializeField] private Button buttonclose;
    [SerializeField] private Transform panelRefresh;
    [Header("Effect Settings")]
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private float flashSpeed = 8f;
    [SerializeField] private UI_PanelStoreDetail detailStore;

    private bool isRefreshing = false;
    private Coroutine buildRoutine;

    void OnEnable()
    {
        if (buildRoutine != null)
            StopCoroutine(buildRoutine);

        buildRoutine = StartCoroutine(CoBuildThenShow());
    }

    private IEnumerator CoBuildThenShow()
    {
        if (contentItems != null)
            contentItems.gameObject.SetActive(false);

        ShopPurchaseState.EnsureInstance();

        yield return null;

        BuildShopFromStateOrCreate();
        if (contentItems != null)
            contentItems.gameObject.SetActive(true);

        buttonRefresh.onClick.RemoveAllListeners();
        buttonRefresh.onClick.AddListener(() =>
        {
            CheckRefresh();
            panelRefresh.gameObject.SetActive(true);
        });

        buttonclose.onClick.RemoveAllListeners();
        buttonclose.onClick.AddListener(() => ClosePanelRefresh());

        buttonOk.onClick.RemoveAllListeners();
        buttonOk.onClick.AddListener(() =>
        {
            if (!isRefreshing)
                StartCoroutine(RefreshShop());
        });

        if (TimeManager.Instance.ShouldResetShop())
        {
            StartCoroutine(RefreshShop());
        }

        buildRoutine = null;
    }

    private void OnDisable()
    {
        if (buildRoutine != null)
        {
            StopCoroutine(buildRoutine);
            buildRoutine = null;
        }

        if (contentItems != null)
            contentItems.gameObject.SetActive(true);
    }
    void BuildShopFromStateOrCreate()
    {
        var state = ShopPurchaseState.Instance;
        if (state == null)
            return;

        if (!state.HasCurrentShop())
        {
            var list = GenerateShopItemIdsForCurrentChapter();
            state.SetCurrentShopItemIds(list);
        }

        ClearOldItems();
        BuildShop(state.GetCurrentShopItemIds());
    }

    IEnumerator RefreshShop()
    {
        var state = ShopPurchaseState.Instance;
        if (state != null)
        {
            var list = GenerateShopItemIdsForCurrentChapter();
            state.SetCurrentShopItemIds(list);
            state.ResetSoldOnly();
        }

        detailStore.gameObject.SetActive(false);
        PlayerInventory.Instance.ConsumeItem(2, 50);
        panelRefresh.gameObject.SetActive(false);
        isRefreshing = true;
        buttonRefresh.interactable = false;

        float duration = rotateDuration;
        float timer = 0f;

        int childCount = contentItems.childCount;

        Quaternion[] startRot = new Quaternion[childCount];
        UI_StoreItem[] items = new UI_StoreItem[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform t = contentItems.GetChild(i);
            startRot[i] = t.rotation;

            items[i] = t.GetComponent<UI_StoreItem>();

            if (items[i] != null)
                StartCoroutine(items[i].PlayGlow(duration));
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            for (int i = 0; i < childCount; i++)
            {
                Transform item = contentItems.GetChild(i);
                item.rotation = startRot[i] * Quaternion.Euler(0, 360f * t, 0);
            }

            yield return null;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform item = contentItems.GetChild(i);
            item.rotation = startRot[i];
        }

        BuildShopFromStateOrCreate();

        buttonRefresh.interactable = true;
        isRefreshing = false;
    }

    List<int> GenerateShopItemIdsForCurrentChapter()
    {
        var list = new List<int>(16);

        list.Add(1);  
        int chapter = ProgressManager.Instance.GetMaxChapterReached();
        if (chapter == 1)
        {
            list.Add(5);
            list.Add(6);
            list.Add(7);
        }
        else if (chapter == 2)
        {
            list.Add(8);
            list.Add(9);
            list.Add(10);
        }
        else if (chapter == 3)
        {
            list.Add(11);
            list.Add(12);
            list.Add(13);
        }
        else if (chapter == 4)
        {
            list.Add(14);
            list.Add(15);
            list.Add(16);
        }

        if (chapter == 2)
        {
            list.Add(5);
            list.Add(6);
            list.Add(7);
        }
        else if (chapter == 3)
        {
            list.Add(8);
            list.Add(9);
            list.Add(10);
        }
        else if (chapter == 4)
        {
            list.Add(11);
            list.Add(12);
            list.Add(13);
        }
        else if (chapter == 5)
        {
            list.Add(14);
            list.Add(15);
            list.Add(16);
        }
        list.Add(UnityEngine.Random.Range(2, 5));

        list.Add(UnityEngine.Random.Range(18, 21));

        list.Add(17);

        return list;
    }

    void BuildShop(IReadOnlyList<int> shopItemIds)
    {
        if (shopItemIds == null) return;

        for (int i = 0; i < shopItemIds.Count; i++)
            SetUpItem(shopItemIds[i]);
    }

    void CheckRefresh()
    {
        if (PlayerInventory.Instance.GetItemQuantity(2) < 50)
        {
            buttonOk.interactable = false;
            return;
        }
        else
        {
            buttonOk.interactable = true;
        }
    }

    void ClosePanelRefresh()
    {
        panelRefresh.gameObject.SetActive(false);
    }

    public void SetUpItem(int shopItemId)
    {
        ShopItemData shopItemData =
            DatabaseManager.Instance.ShopItemDatabase.GetShopItemDataByShopItemId(shopItemId);

        if (shopItemData == null)
        {
            Debug.LogWarning($"[Shop] Missing ShopItemData for shopItemId={shopItemId}");
            return;
        }

        ItemData itemData =
            DatabaseManager.Instance.ItemDatabase.GetItem(shopItemData.itemId);

        if (itemData == null)
        {
            Debug.LogWarning($"[Shop] Missing ItemData for itemId={shopItemData.itemId} (shopItemId={shopItemId})");
            return;
        }

        var go = Instantiate(storeItemPrefabs, contentItems);
        var ui = go.GetComponent<UI_StoreItem>();

        if (ui != null)
            ui.SetUp(itemData, shopItemData);
    }

    void ClearOldItems()
    {
        foreach (Transform child in contentItems)
            Destroy(child.gameObject);
    }

    private void Update()
    {
        if (TimeManager.Instance.ShouldResetShop())
        {
            StartCoroutine(RefreshShop());
        }
    }
}