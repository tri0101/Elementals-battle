using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public List<HeroInstance> heroes = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddHero(int heroId)
    {
        HeroInstance hero = heroes.Find(h => h.heroId == heroId);

        if (hero == null)
        {
            heroes.Add(new HeroInstance
            {
                heroId = heroId,
                level = 1,
                star = 1,
                shard = 0
            });
        }
        else
        {
            hero.shard += 10; // ví dụ trùng → + shard
        }
    }
}
