using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UI_ListSweep : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI battleText;
    public TextMeshProUGUI coinText;
    [Header("Database")]
    public ItemDatabase itemDatabase;
    [Header("Transform")]
    public Transform contentItem;
    public GameObject itemPrefab;
    public GameObject itemPrefabShard;
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
    void SetUpItems()
    {
        foreach (var itemID in itemDrop.Keys)
        {
            if (itemID == 1) continue; //skip coin
            ItemData itemData = itemDatabase.GetItem(itemID);
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
