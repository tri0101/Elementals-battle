using System.Collections.Generic;
using UnityEngine;

public class GachaBannerRuntime
{
    List<HeroGachaData> tierD = new();
    List<HeroGachaData> tierC = new();
    List<HeroGachaData> tierB = new();
    List<HeroGachaData> tierS = new();

    private readonly List<ItemGachaData> items = new();
    public GachaBannerRuntime(GachaBanner banner)
    {
        foreach (var hero in banner.pool)
        {
            switch (hero.tier)
            {
                case HeroTier.D: tierD.Add(hero); break;
                case HeroTier.C: tierC.Add(hero); break;
                case HeroTier.B: tierB.Add(hero); break;
                case HeroTier.S: tierS.Add(hero); break;
            }
        }
    }

    public int RollHero(HeroTier tier)
    {
        var list = tier switch
        {
            HeroTier.D => tierD,
            HeroTier.C => tierC,
            HeroTier.B => tierB,
            HeroTier.S => tierS,
            _ => null
        };

        int index = Random.Range(0, list.Count);
        return list[index].heroId;
    }
    public ItemGachaData RollItem()
    {
        if (items == null || items.Count == 0)
            return null;

        float total = 0f;
        for (int i = 0; i < items.Count; i++)
            total += Mathf.Max(0f, items[i].weight);

        if (total <= 0f)
            return items[Random.Range(0, items.Count)];

        float r = Random.value * total;
        float acc = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            acc += Mathf.Max(0f, items[i].weight);
            if (r <= acc)
                return items[i];
        }

        return items[items.Count - 1];
    }
}
