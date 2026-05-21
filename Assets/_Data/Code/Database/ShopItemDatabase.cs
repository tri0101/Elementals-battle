using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/Shop Database")]
public class ShopItemDatabase : ScriptableObject
{
    public List<ShopItemData> shopItems;

    private Dictionary<int, ShopItemData> shopItemByShopItemId;
    private Dictionary<int, ShopItemData> shopItemByItemId;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        shopItemByShopItemId = new Dictionary<int, ShopItemData>();
        shopItemByItemId = new Dictionary<int, ShopItemData>();

        if (shopItems == null) return;

        foreach (var shopItem in shopItems)
        {
            if (shopItem == null) continue;

            if (shopItem.shopItemId > 0)
                shopItemByShopItemId[shopItem.shopItemId] = shopItem;
            if (shopItem.itemId > 0)
                shopItemByItemId[shopItem.itemId] = shopItem;
        }
    }


    public ShopItemData GetShopItemDataByShopItemId(int shopItemId)
    {
        if (shopItemByShopItemId == null) Init();
        return shopItemByShopItemId != null && shopItemByShopItemId.TryGetValue(shopItemId, out var shopItem)
            ? shopItem
            : null;
    }

    public ShopItemData GetShopItemDataByItemId(int itemId)
    {
        if (shopItemByItemId == null) Init();
        return shopItemByItemId != null && shopItemByItemId.TryGetValue(itemId, out var shopItem)
            ? shopItem
            : null;
    }

    public ShopItemData GetShopItemData(int itemId) => GetShopItemDataByItemId(itemId);

    public ShopItemData GetShopItemDataID(int itemId) => GetShopItemDataByItemId(itemId);
}