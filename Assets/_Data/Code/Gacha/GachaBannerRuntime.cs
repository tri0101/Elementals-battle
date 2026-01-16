using System.Collections.Generic;
using UnityEngine;

public class GachaBannerRuntime
{
    List<HeroGachaData> tierD = new();
    List<HeroGachaData> tierC = new();
    List<HeroGachaData> tierB = new();
    List<HeroGachaData> tierS = new();

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
}
