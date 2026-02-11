using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StageReward : MonoBehaviour
{

    bool panelIsActive = false;
    bool isShowingStar;
    bool isShowingItem;
    [Header("UI")]
    public TextMeshProUGUI conditionText;
    public StageConfig stageConfig;
    public Transform starRoot;
    [Header("Database")]
    public ItemDatabase itemDatabase;
    [Header("Transform")]
    public Transform contentItem;
    public GameObject itemPrefab;
    public GameObject itemPrefabShard;
    [Header("Star Colors")]
    public Color earnedColor = new Color32(255, 215, 0, 255);      // vàng
    public Color notEarnedColor = new Color32(158, 101, 101, 255); // tối

    [Header("Star Effect")]
    public float delayBetweenStars = 0.4f;
    public float flashDuration = 0.15f;
    public float flashScale = 1.25f;
    public int starGain = 0;
    [Header("Item Effect")]
    public float itemDelay = 0.3f;
    public float itemFlashDuration = 0.15f;
    public float itemFlashScale = 1.2f;
    private Dictionary<int, int> itemDrop = new Dictionary<int, int>(); // itemID , amount
    private void OnEnable()
    {
        panelIsActive = true;
        stageConfig = StageContext.selectedStage;
        SetTextCondition();
        DisPlayStar(starGain);
        DisPlayItems();
    }
    public void SetStarGain(int value)
    {
        starGain = value;
    }
    public void SetTextCondition()
    {
        switch (stageConfig.stageCondition)
        {
            case StageCondition.HeroLost:
                int heroLostParam = stageConfig.conditionParam;
                conditionText.text = "Heroes Lost <= " + heroLostParam;
                break;
        }
    }

    // 🔥 GIỮ NGUYÊN HÀM NÀY
    public void DisPlayStar(int starReward)
    {
        isShowingStar = true;
        StopAllCoroutines();
        StartCoroutine(ShowStarsSequentially(starReward));
        
    }

    private IEnumerator ShowStarsSequentially(int starReward)
    {
        // Reset toàn bộ sao về trạng thái chưa đạt
        foreach (Transform child in starRoot)
        {
            Image img = child.GetComponent<Image>();
            if (img == null) continue;

            img.color = notEarnedColor;
            child.localScale = Vector3.one;
        }

        int index = 0;

        foreach (Transform child in starRoot)
        {
            if (index >= starReward)
                break;

            Image img = child.GetComponent<Image>();
            if (img == null) continue;

            // Đổi sang màu vàng
            img.color = earnedColor;

            // Chớp scale
            yield return StartCoroutine(FlashStar(child));

            // Đợi rồi mới bật sao tiếp
            yield return new WaitForSeconds(delayBetweenStars);

            index++;
        }
        isShowingStar = false; isShowingStar = false;
    }

    private IEnumerator FlashStar(Transform star)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * flashScale;

        float time = 0f;

        // Phóng to
        while (time < flashDuration)
        {
            star.localScale = Vector3.Lerp(originalScale, targetScale, time / flashDuration);
            time += Time.deltaTime;
            yield return null;
        }

        star.localScale = targetScale;

        time = 0f;

        // Thu nhỏ lại
        while (time < flashDuration)
        {
            star.localScale = Vector3.Lerp(targetScale, originalScale, time / flashDuration);
            time += Time.deltaTime;
            yield return null;
        }

        star.localScale = originalScale;
    }
    public void SetItemDrop(Dictionary<int, int > itemDrops)
    {
        itemDrop = itemDrops;
    }
   
    
    void ClearItems()
    {
        for (int i = contentItem.childCount - 1; i >= 0; i--)
            Destroy(contentItem.GetChild(i).gameObject);
    }
    void CreateItem(ItemData data, int amount)
    {
        GameObject prefab =
            data.type == ItemType.HeroShard
            ? (itemPrefabShard ?? itemPrefab)
            : itemPrefab;

        var go = Instantiate(prefab, contentItem);
        var ui = go.GetComponent<UI_DropRewardItem>();
        if (ui != null)
            ui.Setup(data, amount);
    }
    public void DisPlayItems()
    {
        isShowingItem = true;
        StopCoroutine("ShowItemsSequentially");
        StartCoroutine(ShowItemsSequentially());
       
    }
    private IEnumerator ShowItemsSequentially()
    {
        ClearItems();

        List<Transform> createdItems = new List<Transform>();

        // 1️⃣ Tạo toàn bộ item nhưng tắt đi
        foreach (var itemID in itemDrop.Keys)
        {
            ItemData itemData = itemDatabase.GetItem(itemID);
            int amount = itemDrop[itemID];
            if (itemData == null) continue;

            GameObject prefab =
                itemData.type == ItemType.HeroShard
                ? (itemPrefabShard ?? itemPrefab)
                : itemPrefab;

            var go = Instantiate(prefab, contentItem);
            go.SetActive(false);

            var ui = go.GetComponent<UI_DropRewardItem>();
            if (ui != null)
                ui.Setup(itemData, amount);

            createdItems.Add(go.transform);
        }

        // 2️⃣ Hiển thị từng item một
        foreach (var item in createdItems)
        {
            item.gameObject.SetActive(true);

            yield return StartCoroutine(FlashItem(item));

            yield return new WaitForSeconds(itemDelay);
        }
        isShowingItem = false;
    }
    private IEnumerator FlashItem(Transform item)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * itemFlashScale;

        float time = 0f;

        // Phóng to
        while (time < itemFlashDuration)
        {
            item.localScale = Vector3.Lerp(originalScale, targetScale, time / itemFlashDuration);
            time += Time.deltaTime;
            yield return null;
        }

        item.localScale = targetScale;

        time = 0f;

        // Thu nhỏ lại
        while (time < itemFlashDuration)
        {
            item.localScale = Vector3.Lerp(targetScale, originalScale, time / itemFlashDuration);
            time += Time.deltaTime;
            yield return null;
        }

        item.localScale = originalScale;
    }
    private void Update()
    {
        if (panelIsActive && !isShowingStar && !isShowingItem)
        {
            if (Input.GetMouseButtonDown(0))
            {
                panelIsActive = false;
                gameObject.SetActive(false);
                
                GameManager.Instance.LoadAdditiveScene(SceneId.MapScene);
                GameManager.Instance.SetCameraActive(true);
                GameManager.Instance.UnLoadAdditiveScene(SceneId.BattleScene);  
            }
        }
    }
}
