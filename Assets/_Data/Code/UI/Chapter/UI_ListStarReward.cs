using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
public class UI_ListStarReward : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] ChapterDatabase chapterDatabase;
    [SerializeField] private ItemDatabase itemDatabase;
    [Header("UI")]
    [SerializeField] private Transform contentItem;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPrefabShard;
    [SerializeField] private TextMeshProUGUI starText;
    void LoadItems(int chapterID, int index)
    {
        LoadText(chapterID, index);
        ClearItems();
        ChapterConfig chapterConfig = chapterDatabase.GetChapter(chapterID);
        if(index == 0)
            foreach (var rewardItems in chapterConfig.dropItems0)
            {
                ItemData itemData = itemDatabase.GetItem(rewardItems.itemId);
                if (itemData == null) continue;
                CreateItem(itemData, rewardItems.amount);
            }
        else if(index == 1)
            foreach (var rewardItems in chapterConfig.dropItems1)
            {
                ItemData itemData = itemDatabase.GetItem(rewardItems.itemId);
                if (itemData == null) continue;
                CreateItem(itemData, rewardItems.amount);
            }
        else if(index == 2)
            foreach (var rewardItems in chapterConfig.dropItems2)
            {
            
            ItemData itemData = itemDatabase.GetItem(rewardItems.itemId);
            if (itemData == null) continue;

            CreateItem(itemData, rewardItems.amount);
            }
    }
    public void OnClick(int chapterID, int indexStar)// 0 = 10 sao, 1 = 20 sao, 2 = 30 sao
    {
        LoadItems(chapterID, indexStar);
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
  
    void ClearItems()
    {
        for (int i = contentItem.childCount - 1; i >= 0; i--)
            Destroy(contentItem.GetChild(i).gameObject);
    }
    void LoadText(int chapterID, int index)
    {
        int starGet = ProgressManager.Instance.GetStarInChapter(chapterID);
        if (index == 0)
        {
            starText.text = $"{starGet}" + "/10";
        }
        else if (index == 1)
        {
            starText.text = $"{starGet}" + "/20";
        }
        else if (index == 2)
        {
            starText.text = $"{starGet}" + "/30";
        }
    }
   
}
