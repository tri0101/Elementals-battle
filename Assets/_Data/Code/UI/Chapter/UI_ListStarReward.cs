using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;
public class UI_ListStarReward : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private Transform contentItem;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPrefabShard;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private Button buttonClaim;
    [SerializeField] private Button buttonClose;
    [SerializeField] private UI_PanelReward panelReward;

    private void Awake()
    {
        buttonClose.onClick.AddListener(() => gameObject.SetActive(false));
    }
    void LoadItems(int chapterID, int index)
    {
        LoadText(chapterID, index);
        LoadButton(chapterID, index);
        ClearItems();
        ChapterConfig chapterConfig = DatabaseManager.Instance.ChapterDatabase.GetChapter(chapterID);
        if (index == 0)
            foreach (var rewardItems in chapterConfig.dropItems0)
            {
                ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(rewardItems.itemId);
                if (itemData == null) continue;
                CreateItem(itemData, rewardItems.amount);
            }
        else if (index == 1)
            foreach (var rewardItems in chapterConfig.dropItems1)
            {
                ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(rewardItems.itemId);
                if (itemData == null) continue;
                CreateItem(itemData, rewardItems.amount);
            }
        else if (index == 2)
            foreach (var rewardItems in chapterConfig.dropItems2)
            {

                ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(rewardItems.itemId);
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
        buttonClaim.onClick.RemoveAllListeners();
        buttonClaim.onClick.AddListener(() => OnClickButtonClaim(chapterID, index));
        
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
    void OnClickButtonClaim(int chapterID, int index)
    {
        ChapterConfig chapterConfig = DatabaseManager.Instance.ChapterDatabase.GetChapter(chapterID);
        UI_CanvasReward.Instance.gameObject.SetActive(true);
        UI_CanvasReward.Instance.ClearOldItems();
        if (index == 0)
        {
            ProgressManager.Instance.SetClaim(chapterID, 0);
            foreach (var rewardItems in chapterConfig.dropItems0)
            {
                
                ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(rewardItems.itemId);
                UI_CanvasReward.Instance.SetUp(itemData, rewardItems.amount);
            }
        }
        else if (index == 1)
        {
            ProgressManager.Instance.SetClaim(chapterID, 1);
            foreach (var rewardItems in chapterConfig.dropItems1)
            {
                
                ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(rewardItems.itemId);
                UI_CanvasReward.Instance.SetUp(itemData, rewardItems.amount);
            }
        }
        else if (index == 2)
        {
            ProgressManager.Instance.SetClaim(chapterID, 2);
            foreach (var rewardItems in chapterConfig.dropItems2)
            {
                
                ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(rewardItems.itemId);
                UI_CanvasReward.Instance.SetUp(itemData, rewardItems.amount);
            }
        }

        UI_CanvasReward.Instance.ShowReward();
        buttonClaim.interactable = false;
        buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
        panelReward.LoadUI(chapterID);
        gameObject.SetActive(false);
    }
    void LoadButton(int chapterID, int index)
    {
        ChapterConfig chapterConfig = DatabaseManager.Instance.ChapterDatabase.GetChapter(chapterID);
        int starGet = ProgressManager.Instance.GetStarInChapter(chapterID);
        bool isClaimed = ProgressManager.Instance.IsClaimed(chapterID, index);
        if ((starGet < 10 && index == 0) || isClaimed)
        {
            buttonClaim.interactable = false;
            buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
        }
        else if ((starGet < 20 && index == 1) || isClaimed)
        {
            buttonClaim.interactable = false;
            buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
        }
        else if ((starGet < 30 && index == 2) || isClaimed)
        {
            buttonClaim.interactable = false;
            buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
        }
        else
        {
            buttonClaim.interactable = !isClaimed;
            buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";

        }
    }
}
