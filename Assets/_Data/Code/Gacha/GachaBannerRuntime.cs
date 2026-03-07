using System.Collections.Generic;
using UnityEngine;

public class GachaBannerRuntime
{
    List<HeroGachaData> tierC = new();
    List<HeroGachaData> tierB = new();
    List<HeroGachaData> tierA = new();
    List<HeroGachaData> tierS = new();
    List<HeroGachaData> tierSS = new();

    private readonly List<ItemGachaData> items = new();

    public GachaBannerRuntime(GachaBanner banner)
    {
        if (banner == null || banner.pool == null)
            return;

        foreach (var hero in banner.pool)
        {
            switch (hero.tier)
            {
                case HeroTier.C: tierC.Add(hero); break;
                case HeroTier.B: tierB.Add(hero); break;
                case HeroTier.A: tierA.Add(hero); break;
                case HeroTier.S: tierS.Add(hero); break;
                case HeroTier.SS: tierSS.Add(hero); break;
            }
        }

        if (banner.itemPool != null)
            items.AddRange(banner.itemPool);
    }

    public bool HasTier(HeroTier tier)
    {
        return tier switch
        {
            HeroTier.C => tierC.Count > 0,
            HeroTier.B => tierB.Count > 0,
            HeroTier.A => tierA.Count > 0,
            HeroTier.S => tierS.Count > 0,
            HeroTier.SS => tierSS.Count > 0,
            _ => false
        };
    }

    public int RollHero(HeroTier tier)
    {
        var list = tier switch
        {
            HeroTier.C => tierC,
            HeroTier.B => tierB,
            HeroTier.A => tierA,
            HeroTier.S => tierS,
            HeroTier.SS => tierSS,
            _ => null
        };

        if (list == null || list.Count == 0)
            return -1;

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