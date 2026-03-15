using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UI_ListSweep : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("Transform")]
    [SerializeField] private Transform contentItem;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPrefabShard;
    private Dictionary<int, int> itemDrop = new Dictionary<int, int>();
    public void SetText(int orderBattle)
    {
        battleText.text = $"Battle {orderBattle}";
    }
    public void SetItemDrop(Dictionary<int, int> itemDrops)
    {
        itemDrop = itemDrops;
        SetUpItems();
        SetCoin();
    }
    void SetCoin()
    {
        coinText.text = itemDrop.ContainsKey(1) ? itemDrop[1].ToString() : "0";
    }
    void SetExp()
    {
        expText.text = AccountManager.Instance.AccountP.exp.ToString();
    }
    void SetUpItems()
    {
        foreach (var itemID in itemDrop.Keys)
        {
            if (itemID == 1) continue; //skip coin
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemID);
            int amount = itemDrop[itemID];
            if (itemData == null) continue;

            GameObject prefab =
                itemData.type == ItemType.HeroShard
                ? (itemPrefabShard ?? itemPrefab)
                : itemPrefab;

            var go = Instantiate(prefab, contentItem);
            go.SetActive(true);

            var ui = go.GetComponent<UI_DropRewardItem>();
            if (ui != null)
                ui.Setup(itemData, amount);

           
        }
    }
}
