using System;
using UnityEngine;
using System.Collections.Generic;

public class UI_ListStoreItem : MonoBehaviour
{
    [SerializeField] private GameObject storeItemPrefabs;
    private const string LAST_RESET_KEY = "LastShopReset";
    private readonly int[] resetHours = { 0, 6, 12, 18 };
    [SerializeField] private Transform contentItems;
    void OnEnable()
    {
        CheckReset();
        BuildShop();
    }
    void CheckReset()
    {
        DateTime now = System.DateTime.Now;
        DateTime currentResetPoint = GetCurrentResetPoint(now);

        string saved = PlayerPrefs.GetString(LAST_RESET_KEY, "");

        if (saved != currentResetPoint.ToString())
        {
            PlayerPrefs.SetString(LAST_RESET_KEY, currentResetPoint.ToString());
            PlayerPrefs.Save();
        }
    }
    DateTime GetCurrentResetPoint(System.DateTime now)
    {
        System.DateTime today = now.Date;
        System.DateTime latest = today;

        foreach (int hour in resetHours)
        {
            System.DateTime point = today.AddHours(hour);

            if (now >= point)
                latest = point;
        }

        return latest;
    }
    void BuildShop()
    {
        ClearOldItems();

        SetUpItem(100);
        GetNeedItems();
        SetUpFood();
        SetUpShard();
        SetUpItem(4);
    }
    void SetUpFood()
    {
        int number = UnityEngine.Random.Range(50, 53);
        SetUpItem(number);
    }
    void SetUpShard()
    {
        int number = UnityEngine.Random.Range(1001, 1004);
        SetUpItem(number);
    }
    void GetNeedItems()
    {
        if (ProgressManager.Instance.GetChapter() == 1)
        {
            SetUpItem(101);
            SetUpItem(102);
            SetUpItem(103);
        }
        else if (ProgressManager.Instance.GetChapter() == 2)
        {
            SetUpItem(104);
            SetUpItem(105);
            SetUpItem(106);
        }
    }
    public void SetUpItem(int itemId)
    {
        ShopItemData shopItemData = DatabaseManager.Instance.ShopItemDatabase.GetShopItemData(itemId);
        ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemId);
        SetUp(itemData, shopItemData);
    }
    public void SetUp(ItemData itemdata, ShopItemData shopItemData)
    {
        var go = Instantiate(storeItemPrefabs, contentItems);
        var ui = go.GetComponent<UI_StoreItem>();
        if (ui != null)
            ui.SetUp(itemdata, shopItemData);
    }
    void ClearOldItems()
    {
        foreach (Transform child in contentItems)
        {
            Destroy(child.gameObject);
        }
    }
}
