using System.Collections.Generic;
using UnityEngine;
public enum GachaResultType
{
    Hero,
    Shard
}

public struct GachaResult
{
    public int heroId;
    public GachaResultType type;
}
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public List<HeroInstance> heroes = new();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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

}
