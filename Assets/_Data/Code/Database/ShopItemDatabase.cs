using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/Shop Database")]
public class ShopItemDatabase : ScriptableObject
{
    public List<ShopItemData> shopItems;

    private Dictionary<int, ShopItemData> shopItemDict;

    public void Init()
    {
        shopItemDict = new Dictionary<int, ShopItemData
            >();
        foreach (var shopItem in shopItems)
            shopItemDict[shopItem.itemId] = shopItem;
    }

    public ShopItemData GetShopItemData(int id)
    {
        if (shopItemDict == null) Init();

        return shopItemDict.TryGetValue(id, out var shopItem) ? shopItem : null;
    }
}