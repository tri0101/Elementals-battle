using System.Collections.Generic;
using UnityEngine;

public class UI_ListRankSourceUpgrade : MonoBehaviour
{
    [Header("Config")]
    public HeroRankConfig rankConfig;
    public ItemDatabase itemDatabase;

    [Header("UI")]
    public Transform content;
    public GameObject itemPrefab;

    HeroViewData currentHero;

    // ================= ENTRY =================

    public void Setup(HeroViewData hero)
    {
        currentHero = hero;
        Build();
    }

    void Build()
    {
        Clear();
        if (currentHero == null) return;

        RankRequirement req = rankConfig.rankRequirements
            .Find(r => r.rank == currentHero.instance.rank);

        if (req == null)
        {
            Debug.LogWarning($"No RankRequirement for rank {currentHero.instance.rank}");
            return;
        }

        // ===== SLOT 1 : SOURCE CORE =====
        CreateSlot(100, GetRequiredAmount(req, 100));

        // ===== SLOT 2 : MAIN ROLE (x2) =====
        CreateSlotByRole(req, currentHero.info.role, true);

        // ===== SLOT 3 & 4 : SECONDARY =====
        CreateSlotByRole(req, currentHero.info.role, false);
    }

    // ================= SLOT =================

    void CreateSlot(int itemId, int required)
    {
        ItemData itemData = itemDatabase.GetItem(itemId);
        if (itemData == null) return;

        int owned = GetOwned(itemId);

        var go = Instantiate(itemPrefab, content);
        go.GetComponent<UI_RankSourceUpgradeItem>()
          .Setup(itemData, owned, required);
    }

    void CreateSlotByRole(RankRequirement req, RoleHero role, bool main)
    {
        List<string> keys = main
            ? GetMainKeys(role)
            : GetSecondaryKeys(role);

        int multiplier = main ? 2 : 1;

        foreach (string key in keys)
        {
            ItemCost cost = req.costs.Find(c =>
            {
                ItemData data = itemDatabase.GetItem(c.itemId);
                return data != null && data.name.Contains(key);
            });

            if (cost == null) continue;

            CreateSlot(cost.itemId, cost.amount * multiplier);
        }
    }

    // ================= DATA =================

    int GetRequiredAmount(RankRequirement req, int itemId)
    {
        ItemCost cost = req.costs.Find(c => c.itemId == itemId);
        return cost != null ? cost.amount : 0;
    }

    int GetOwned(int itemId)
    {
        ItemInstance item = PlayerInventory.Instance.items
            .Find(i => i.itemId == itemId);

        return item != null ? item.quantity : 0;
    }

    void Clear()
    {
        foreach (Transform c in content)
            Destroy(c.gameObject);
    }

    // ================= ROLE MAP =================

    List<string> GetMainKeys(RoleHero role)
    {
        switch (role)
        {
            case RoleHero.Tank:
                return new() { "Shield" };

            case RoleHero.DPS:
                return new() { "Sword" };

            case RoleHero.Support:
                return new() { "Star" };

            default:
                return new();
        }
    }

    List<string> GetSecondaryKeys(RoleHero role)
    {
        switch (role)
        {
            case RoleHero.Tank:
                return new() { "Sword", "Star" };

            case RoleHero.DPS:
                return new() { "Shield", "Star" };

            case RoleHero.Support:
                return new() { "Shield", "Sword" };

            default:
                return new();
        }
    }
}
