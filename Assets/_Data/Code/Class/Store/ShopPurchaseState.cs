using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public sealed class ShopPurchaseState : Subject
{
    public static ShopPurchaseState Instance { get; private set; }

    // shopItemId đã mua trong chu kỳ shop hiện tại
    private readonly HashSet<int> soldShopItemIds = new HashSet<int>();

    // shopItemId đang được bày bán trong chu kỳ hiện tại (theo slot)
    private readonly List<int> currentShopItemIds = new List<int>(16);

    public static ShopPurchaseState EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        var go = new GameObject(nameof(ShopPurchaseState));
        Instance = go.AddComponent<ShopPurchaseState>();
        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsSold(int shopItemId) => soldShopItemIds.Contains(shopItemId);

    public void MarkSold(int shopItemId)
    {
        if (shopItemId <= 0) return;
        soldShopItemIds.Add(shopItemId);
        NotifyObservers();
    }

    public void SetSoldShopItemIds(List<int> shopItemIds)
    {
        foreach (int id in shopItemIds)
        {
            if (id > 0)
                soldShopItemIds.Add(id);
        }
        NotifyObservers();
    }
    public void ResetCycle()
    {
        soldShopItemIds.Clear();
        currentShopItemIds.Clear();
    }

    public bool HasCurrentShop()
    {
        return currentShopItemIds.Count > 0;
    }

    public IReadOnlyList<int> GetCurrentShopItemIds()
    {
        return currentShopItemIds;
    }
    public List<int> GetSoldShopItemIds()
    {
        return new List<int>(soldShopItemIds);
    }
    public IReadOnlyCollection<int> GetSoldShopItemIdsCollection()
    {
        return soldShopItemIds;
    }
    public void SetCurrentShopItemIds(List<int> shopItemIds)
    {
        currentShopItemIds.Clear();
        if (shopItemIds == null) return;

        for (int i = 0; i < shopItemIds.Count; i++)
        {
            int id = shopItemIds[i];
            if (id > 0)
                currentShopItemIds.Add(id);
        }
        NotifyObservers();
    }

    public void ResetSoldOnly()
    {
        soldShopItemIds.Clear();
    }
}