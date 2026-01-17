using UnityEngine;
using System.Collections.Generic;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance { get; private set; }
    public GachaBanner banner;
    private GachaBannerRuntime runtime;

    public int pullCount;
    private const int PITY_S = 50;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        runtime = new GachaBannerRuntime(banner);
    }

    public GachaResult Roll()
    {
        pullCount++;

        int heroId;

        if (pullCount >= PITY_S)
        {
            pullCount = 0;
            heroId = runtime.RollHero(HeroTier.S);
        }
        else
        {
            HeroTier tier = RollTierNormal();
            heroId = runtime.RollHero(tier);
        }

        return PlayerInventory.Instance.AddHero(heroId);
    }

    public List<GachaResult> RollTen()
    {
        List<GachaResult> results = new List<GachaResult>();

        for (int i = 0; i < 10; i++)
        {
            GachaResult result = Roll();
            results.Add(result);
        }

        return results;
    }


    HeroTier RollTierNormal()
    {
        float rand = Random.value;

        if (rand < 0.6f) return HeroTier.D;
        if (rand < 0.9f) return HeroTier.C;
        return HeroTier.B;
    }
}
