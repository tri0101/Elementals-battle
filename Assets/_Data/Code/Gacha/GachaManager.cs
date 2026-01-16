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
    public void OnClickRoll()
    {
        int x = Roll();
    }
    public void OnClickRollTen()
    {
        List<int> heroIds = RollTen();
        
    }
    public int Roll()
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

        // 👉 ADD VÀO INVENTORY Ở ĐÂY
        PlayerInventory.Instance.AddHero(heroId);

        return heroId;
    }
    public List<int> RollTen()
    {
        List<int> results = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            int heroId = Roll(); 
            results.Add(heroId);
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
