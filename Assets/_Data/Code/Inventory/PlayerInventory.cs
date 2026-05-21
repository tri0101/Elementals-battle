using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInventory :  Subject
{
    public static PlayerInventory Instance;
    [SerializeField] private List<HeroInstance> heroes = new();
    public IReadOnlyList<HeroInstance> Heroes => heroes;
    [SerializeField] private List<ItemInstance> items = new();
    public IReadOnlyList<ItemInstance> Items => items;

    int maxMana = 150;
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
            if (item.itemId == 1 || item.itemId == 2 || item.itemId == 3)
            {
                NotifyObservers((item.itemId, item.quantity));
            }
        }
    }
    // ================= DEV TOOL =================
    [ContextMenu("DEV / Give Test Items")]
    public void GiveTestItems()
    {
        AddItem(7, 150);
        AddItem(6, 150);
        AddItem(200, 10);
        AddItem(201, 10);
        AddItem(202, 10);
        AddItem(203, 10);
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
        AddItem(107, 1000); 
        AddItem(108, 1000); 
        AddItem(109, 1000); 
        AddItem(110, 1000); 
        AddItem(111, 1000); 
        AddItem(112, 1000); 
        AddItem(1, 500); 
        AddItem(2, 25000); 
        AddItem(3, 1); 
        AddItem(4, 25);
        AddItem(5, 400);

    }
    // ================= INVENTORY =================
    //set and get  ( dùng để lưu)
    public void SetItems(List<ItemInstance> loadedItems)
    {
        items = new List<ItemInstance>(loadedItems);

        items.FindAll(i => i.itemId == 1 || i.itemId == 2 ||  i.itemId == 3).ForEach(i => NotifyObservers((i.itemId, i.quantity)));
    }
    public void SetHeroes(List<HeroInstance> loadedHeroes)
    {
        heroes = new List<HeroInstance>(loadedHeroes);

        
    }
    public HeroInstance FindHeroInstance(int id)
    {
        return heroes.Find(h => h.heroId == id);
    }
    public List<ItemInstance> GetAllItems()
    {
        return items;
    }
    public List<HeroInstance> GetAllHeroes()
    {
        return heroes;
    }




    public GachaResult AddHero(int heroId)
    {
        HeroInstance hero = heroes.Find(h => h.heroId == heroId);
        HeroInfo heroInfo = DatabaseManager.Instance.HeroDatabase.GetHero(heroId);
        if (hero == null)
        {
            hero = new HeroInstance
            {
                heroId = heroId,
                level = 1,
                currentExp = 0,
                speedLevel = 1,
                currentSpeedExp = 0,
                star = 4,
                rank = 1,
                shard = 0
            };

            hero.InitSkillInstances();
            
            hero.AddFightSoul(heroInfo.soulID[0]);
            hero.AddFightSoul(heroInfo.soulID[1]);
            hero.AddFightSoul(heroInfo.soulID[2]);
            heroes.Add(hero);
            NotifyObservers();
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
            NotifyObservers();
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

        if (itemId == 1 || itemId == 2 || itemId == 3 || itemId == 5 || itemId == 6 || itemId == 7)
        {
            NotifyObservers((itemId, item.quantity));
        }

        NotifyObservers();
    }
    public bool ConsumeItem(int itemId, int amount)
    {
        var item = items.Find(i => i.itemId == itemId);
        if (item == null || item.quantity < amount)
            return false;

        item.quantity -= amount;
        if (itemId == 1 || itemId == 2 || itemId == 3 || itemId == 5 || itemId == 6 || itemId == 7)
        {
            if(itemId == 3)
            {
                DailyTaskManager.Instance.AddProgress(3, amount);

            }
            NotifyObservers((itemId, item.quantity));

        }
        NotifyObservers();
        return true;
    }
    public int GetItemQuantity(int itemId)
    {
        var item = items.Find(i => i.itemId == itemId);
        if (item == null)
            return 0;
        return item.quantity;
    }
    public HeroInstance GetHeroInstance(int heroId)
    {
        return heroes.Find(h => h.heroId == heroId);
    }



    //=================Time =================
    void Update()
    {
        RegenMana();
    }
    void RegenMana()
    {
        int currentMana = GetItemQuantity(3);

        if (currentMana >= maxMana)
            return;

        int regenInterval = 3;

        int secondsPassed = TimeManager.Instance.GetManaSecondsPassed();

        while (secondsPassed >= regenInterval)
        {
            AddItem(3, 1);

            TimeManager.Instance.AdvanceManaTime(regenInterval);

            secondsPassed -= regenInterval;
        }
    }
    public int GetHeroCount()
    {
        return heroes.Count;
    }
}


