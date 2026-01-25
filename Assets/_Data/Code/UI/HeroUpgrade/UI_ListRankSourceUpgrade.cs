using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_ListRankSourceUpgrade : MonoBehaviour
{
    [Header("Config")]
    public HeroRankConfig rankConfig;
    public ItemDatabase itemDatabase;

    [Header("UI")]
    public Transform content;
    public GameObject itemPrefab;
    public TextMeshProUGUI amountTextCoin;
    public Button upgradeButton;
    public UI_ListHeroUpgrade uiHeroUpgrade;
    HeroViewData currentHero;

    // ================= ENTRY =================

    public void Setup(HeroViewData hero)
    {
        currentHero = hero;

        // Wire upgrade button
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

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
            if (amountTextCoin != null) amountTextCoin.text = "-";
            if (upgradeButton != null) upgradeButton.interactable = false;
            return;
        }

        // ===== SLOT 1 : SOURCE CORE =====
        CreateSlot(100, GetRequiredAmount(req, 100));

        // ===== SLOT 2 : MAIN ROLE (x2) =====
        CreateSlotByRole(req, currentHero.info.role, true);

        // ===== SLOT 3 & 4 : SECONDARY =====
        CreateSlotByRole(req, currentHero.info.role, false);

        // ===== COIN (itemId == 1) =====
        // If RankRequirement has an entry for itemId == 1, display owned/required coin
        ItemCost coinCost = req.costs.Find(c => c.itemId == 1);
        int requiredCoin = coinCost != null ? coinCost.amount : 0;
        int ownedCoin = GetOwned(1);
        if (amountTextCoin != null)
            amountTextCoin.text = $"{requiredCoin}";

        // Enable/disable upgrade button if all requirements (4 items + coin) are met
        var compiled = CompileRequirements(req, currentHero.info.role);
        bool can = CanUpgrade(compiled);
        if (upgradeButton != null)
            upgradeButton.interactable = can;
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

    // ================= UI-side requirement compilation (UI chịu trách nhiệm hiển thị) =================

    // Tạo danh sách ItemCost thực tế (bao gồm coin itemId == 1) theo RankRequirement và role
    List<ItemCost> CompileRequirements(RankRequirement req, RoleHero role)
    {
        var list = new List<ItemCost>();
        if (req == null) return list;

        // slot 1 core
        AddOrAccumulate(list, 100, GetRequiredAmount(req, 100));

        // main x2
        CompileRoleSlotsToList(list, req, role, true);

        // secondary x1
        CompileRoleSlotsToList(list, req, role, false);

        // coin explicit
        ItemCost coin = req.costs.Find(c => c.itemId == 1);
        if (coin != null) AddOrAccumulate(list, 1, coin.amount);

        return list;
    }

    void CompileRoleSlotsToList(List<ItemCost> outList, RankRequirement req, RoleHero role, bool main)
    {
        List<string> keys = main ? GetMainKeys(role) : GetSecondaryKeys(role);
        int multiplier = main ? 2 : 1;

        foreach (string key in keys)
        {
            ItemCost cost = req.costs.Find(c =>
            {
                ItemData data = itemDatabase.GetItem(c.itemId);
                return data != null && data.name.Contains(key);
            });

            if (cost == null) continue;

            AddOrAccumulate(outList, cost.itemId, cost.amount * multiplier);
        }
    }

    void AddOrAccumulate(List<ItemCost> list, int itemId, int amount)
    {
        if (amount <= 0) return;
        var e = list.Find(x => x.itemId == itemId);
        if (e != null) e.amount += amount;
        else list.Add(new ItemCost { itemId = itemId, amount = amount });
    }

    bool CanUpgrade(List<ItemCost> compiled)
    {
        if (compiled == null || compiled.Count == 0) return false;
        foreach (var c in compiled)
        {
            int owned = GetOwned(c.itemId);
            if (owned < c.amount) return false;
        }
        return true;
    }

    // ================= UPGRADE =================

    void OnUpgradeClicked()
    {
        if (currentHero == null) return;

        RankRequirement req = rankConfig.rankRequirements
            .Find(r => r.rank == currentHero.instance.rank);

        if (req == null) return;

        var compiled = CompileRequirements(req, currentHero.info.role);

        // call service to perform consumption + rank increment
        bool ok = HeroUpgradeService.Instance.UpgradeRank(currentHero.instance, compiled);
        if (!ok)
        {
            Debug.Log("Upgrade failed or not enough materials.");
            Build(); // refresh display defensively
            return;
        }

        // Upgrade successful: update UI
        Build();

        // Notify list and header to refresh so UI_HeroUpgradeItem and list reflect new rank/visuals
        if (uiHeroUpgrade != null)
            uiHeroUpgrade.Refresh();
    }
}