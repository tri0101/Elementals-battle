using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class HeroUpgradeService : Subject
{
    public static HeroUpgradeService Instance;
    [SerializeField] private HeroLevelConfig levelConfig;
    public HeroLevelConfig LevelConfig => levelConfig;
    [SerializeField] private HeroSpeedConfig speedConfig;
    public HeroSpeedConfig SpeedConfig => speedConfig;
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Cho hero ăn 1 ExpFood
    /// </summary>
    /// 
    public void FeedExp(HeroInstance hero, int value)
    {
        hero.currentExp += value;
        ProcessLevelUp(hero);
        NotifyObservers();
    }
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
        NotifyObservers();
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
        NotifyObservers();
    }
    public bool FeedSpeedExp(HeroInstance hero, ItemData item)
    {
        // 1. chỉ nhận exp food
        if (item.speedValue <= 0)
            return false;

        // 2. trừ item
        bool consumed = PlayerInventory.Instance.ConsumeItem(item.id, 1);
        if (!consumed) return false;

        // 3. cộng exp
        hero.currentSpeedExp += item.speedValue;

        // 4. xử lý level up
        ProcessSpeedLevelUp(hero);
        NotifyObservers();
        return true;
    }

    void ProcessSpeedLevelUp(HeroInstance hero)
    {
        // tránh out of range
        while (hero.speedLevel - 1 < speedConfig.expPerLevel.Length)
        {
            int needExp = speedConfig.expPerLevel[hero.speedLevel - 1];

            if (hero.currentSpeedExp < needExp)
                break;

            hero.currentSpeedExp -= needExp;
            hero.speedLevel++;
        }
        NotifyObservers();
    }

    public bool UpgradeRank(HeroInstance hero, List<ItemCost> costs)
    {
        if (hero == null || costs == null || costs.Count == 0)
            return false;

 
        foreach (var cost in costs)
        {
            
            var owned = PlayerInventory.Instance.GetItemQuantity(cost.itemId);
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
        NotifyObservers();
        return true;
    }

    public void UpSkill(HeroInstance hero,AbilityType type)
    {
        var skill = hero.skillInstances.Find(s => s.AbilityType == type);
        if (skill != null)
        {
            skill.level++;
        }
        NotifyObservers();
    }


    public void UpSoul(HeroInstance hero, int index)// hero nào , hồn lực thứ mấy
    {
        if (index < 0 || index >= hero.soulsInstances.Count)
            return;
        hero.soulsInstances[index].level++;
        NotifyObservers();
    }
}