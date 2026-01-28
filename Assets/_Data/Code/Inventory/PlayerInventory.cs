using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory :  Subject
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
    public void NotifyCurrentResource()
    {
        foreach (var item in items)
        {
            if (item.itemId == 1 || item.itemId == 2)
            {
                NotifyObservers((item.itemId, item.quantity));
            }
        }
    }
    // ================= DEV TOOL =================
    [ContextMenu("DEV / Give Test Items")]
    public void GiveTestItems()
    {
        AddItem(51, 1000); 
        AddItem(52, 1000); 
        AddItem(55, 1000); 
        AddItem(50, 1000); 
        AddItem(50, 1000); 
        AddItem(100, 1000); 
        AddItem(101, 1000); 
        AddItem(102, 1000); 
        AddItem(103, 1000); 
        AddItem(104, 1000); 
        AddItem(105, 1000); 
        AddItem(106, 1000); 
        AddItem(1, 5000000); 
        AddItem(2, 25000); 
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
                currentExp = 0,
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
            item = new ItemInstance
            {
                itemId = itemId,
                quantity = amount
            };
            items.Add(item);
        }
        else
        {
            item.quantity += amount;
        }

        if (itemId == 1 || itemId == 2)
        {
            NotifyObservers((itemId, item.quantity));
        }
    }
    public bool ConsumeItem(int itemId, int amount)
    {
        var item = items.Find(i => i.itemId == itemId);
        if (item == null || item.quantity < amount)
            return false;

        item.quantity -= amount;
        if (itemId == 1 || itemId == 2)
        {
            NotifyObservers((itemId, item.quantity));
        }
        return true;
    }
}
