using System.Collections.Generic;
using UnityEngine;

public class HeroUpgradeService : MonoBehaviour
{
    public static HeroUpgradeService Instance;
    public HeroLevelConfig levelConfig;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Cho hero ăn 1 ExpFood
    /// </summary>
    public bool FeedExp(HeroInstance hero, ItemData item)
    {
        // 1. chỉ nhận exp food
        if (item.expValue <= 0)
            return false;

        // 2. trừ item
        bool consumed = PlayerInventory.Instance.ConsumeItem(item.id, 1);
        if (!consumed) return false;

        // 3. cộng exp
        hero.currentExp += item.expValue;

        // 4. xử lý level up
        ProcessLevelUp(hero);

        return true;
    }

    void ProcessLevelUp(HeroInstance hero)
    {
        // tránh out of range
        while (hero.level - 1 < levelConfig.expPerLevel.Length)
        {
            int needExp = levelConfig.expPerLevel[hero.level - 1];

            if (hero.currentExp < needExp)
                break;

            hero.currentExp -= needExp;
            hero.level++;
        }
    }

    public bool UpgradeRank(HeroInstance hero, List<ItemCost> costs)
    {
        if (hero == null || costs == null || costs.Count == 0)
            return false;

 
        foreach (var cost in costs)
        {
            var inv = PlayerInventory.Instance.items.Find(i => i.itemId == cost.itemId);
            int owned = inv != null ? inv.quantity : 0;
            if (owned < cost.amount)
            {
                return false;
            }
        }

      
        var consumed = new List<(int itemId, int amount)>();
        foreach (var cost in costs)
        {
            bool ok = PlayerInventory.Instance.ConsumeItem(cost.itemId, cost.amount);
            if (ok)
            {
                consumed.Add((cost.itemId, cost.amount));
            }
            else
            {
                foreach (var c in consumed)
                    PlayerInventory.Instance.AddItem(c.itemId, c.amount);

                return false;
            }
        }

        hero.rank++;

        return true;
    }
}