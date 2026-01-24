using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public List<HeroInstance> heroes = new();
    public List<ItemInstance> items = new();
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
    // ================= DEV TOOL =================
    [ContextMenu("DEV / Give Test Items")]
    public void GiveTestItems()
    {
        AddItem(51, 1000); 
        AddItem(52, 1000); 
        AddItem(50, 1000); 
        AddItem(100, 1000); 
        AddItem(101, 1000); 
        AddItem(103, 1000); 
    }
    // ================= INVENTORY =================
    public GachaResult AddHero(int heroId)
    {
        HeroInstance hero = heroes.Find(h => h.heroId == heroId);

        if (hero == null)
        {
            heroes.Add(new HeroInstance
            {
                heroId = heroId,
                level = 1,
                star = 4,
                rank = 1,
                shard = 0
            });

            return new GachaResult
            {
                heroId = heroId,
                type = GachaResultType.Hero
            };
        }
        else
        {
            hero.shard += 10;
            AddItem(
            itemId: heroId + 1000,           // shard id = hero id + 1000
            amount: 10
        );

            return new GachaResult
            {
                heroId = heroId,
                type = GachaResultType.Shard
            };
        }
    }
    public List<HeroViewData> GetHeroViewList(HeroDatabase db)
    {
        List<HeroViewData> list = new();

        foreach (var hero in heroes)
        {
            HeroInfo info = db.GetHero(hero.heroId);
            if (info == null) continue;

            list.Add(new HeroViewData
            {
                info = info,
                instance = hero
            });
        }

        return list;
    }
    public void AddItem(int itemId, int amount)
    {
        ItemInstance item = items.Find(i => i.itemId == itemId);

        if (item == null)
        {
            items.Add(new ItemInstance
            {
                itemId = itemId,
                quantity = amount
            });
        }
        else
        {
            item.quantity += amount;
        }
    }
    public bool ConsumeItem(int itemId, int amount)
    {
        var item = items.Find(i => i.itemId == itemId);
        if (item == null || item.quantity < amount)
            return false;

        item.quantity -= amount;
        return true;
    }
}
