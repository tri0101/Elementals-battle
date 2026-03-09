using UnityEngine;
using System.Collections.Generic;

public class GachaManager : Subject, IGachaService
{
    public static GachaManager Instance { get; private set; }

    [Header("Banners")]
    [SerializeField] private GachaBanner standardBanner;
    public GachaBanner StandardBanner => standardBanner;
    [SerializeField] private GachaBanner featuredBanner;
    public GachaBanner FeaturedBanner => featuredBanner;
    [SerializeField] private BannerTokenExchange tokenExchangeList;

    [SerializeField] private GachaBanner activeBanner;
    private GachaBannerRuntime runtime;

    [Header("Standard Pity")]
    [SerializeField] private int standardSoftPityStart = 80;
    [SerializeField] private int standardHardPity = 120;
    [SerializeField] private float standardSoftPityChanceAtStart = 0.02f;
    [SerializeField] private float standardSoftPityChanceAtEnd = 1.0f;


    [Header("Featured Pity (SS)")]
    [SerializeField] private int featuredSoftPityStart = 80;
    [SerializeField] private int featuredHardPity = 120;
    [SerializeField] private float featuredSoftPityChanceAtStart = 0.02f;
    [SerializeField] private float featuredSoftPityChanceAtEnd = 1.0f;

    public int pullCount;

    [Header("Stanard State")]

    [SerializeField] int standardPityCounter = 0;
    public int StandardPityCounter => standardPityCounter;
    [Header("Featured State")]
    [SerializeField] private int selectedFeaturedHeroId = -1;
    [SerializeField] private int featuredPityCounter = 0;
    public int FeaturedPityCounter => featuredPityCounter;

    [Header("Item Drop Rate")]
    [Range(0f, 1f)]
    [SerializeField] private float itemDropChance = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetActiveBanner(standardBanner != null ? standardBanner : featuredBanner);
    }
    public void SetPityCounters(int standard, int featured)
    {
        standardPityCounter = Mathf.Max(0, standard);
        featuredPityCounter = Mathf.Max(0, featured);
    }

    void IncStandardPity()
    {
        standardPityCounter++;
        NotifyObservers(); // báo SaveManager save
    }

    void IncFeaturedPity()
    {
        featuredPityCounter++;
        NotifyObservers();
    }

    void ResetStandardPity()
    {
        if (standardPityCounter == 0) return;
        standardPityCounter = 0;
        NotifyObservers();
    }

    void ResetFeaturedPity()
    {
        if (featuredPityCounter == 0) return;
        featuredPityCounter = 0;
        NotifyObservers();
    }

    public void SetActiveBanner(GachaBanner banner)
    {
        activeBanner = banner;
        runtime = activeBanner != null ? new GachaBannerRuntime(activeBanner) : null;

        //pullCount = 0;
        //featuredPityCounter = 0;

        // Optional: auto-pick first featured hero if entering featured banner and none selected
        if (activeBanner != null && activeBanner.bannerType == GachaBannerType.Featured)
        {
            if (selectedFeaturedHeroId <= 0 && featuredBanner != null && featuredBanner.featuredPool != null && featuredBanner.featuredPool.Count > 0)
                selectedFeaturedHeroId = featuredBanner.featuredPool[0];
        }
    }

    public void SetFeaturedSelectionByHeroId(int heroId)
    {
        if (featuredBanner == null || featuredBanner.featuredPool == null || featuredBanner.featuredPool.Count == 0)
        {
            Debug.LogError("Featured banner has no featuredPool configured.");
            return;
        }

        if (!featuredBanner.featuredPool.Contains(heroId))
        {
            Debug.LogError($"HeroId {heroId} is not in featuredPool.");
            return;
        }

        selectedFeaturedHeroId = heroId;
        
    }

    public int GetSelectedFeaturedHeroId()
    {
        return selectedFeaturedHeroId;
    }

    public GachaResult Roll()
    {
        if (runtime == null || activeBanner == null)
        {
            Debug.LogError("GachaManager.Roll failed: activeBanner/runtime is null.");
            return default;
        }
        if (activeBanner.bannerType == GachaBannerType.Featured)
        {
            PlayerInventory.Instance.AddItem(6, 1); // tặng 1 token mỗi lần roll featured 
        }
        pullCount++;
        if (activeBanner.bannerType == GachaBannerType.Featured)
            IncFeaturedPity();
        else IncStandardPity();
        if (activeBanner.itemPool != null && activeBanner.itemPool.Count > 0)
        {
            if (Random.value < itemDropChance)
            {
                var item = runtime.RollItem();
                if (item != null)
                {
                    PlayerInventory.Instance.AddItem(item.itemId, item.amount);

                    return new GachaResult
                    {
                        heroId = -1,
                        itemId = item.itemId,
                        amount = item.amount,
                        type = GachaResultType.Item
                    };
                }
            }
        }

        int heroId = -1;

        if (activeBanner.bannerType == GachaBannerType.Standard)
        {
            heroId = RollStandardBannerHeroId();
            if (heroId > 0 && IsTierS(heroId))
                ResetStandardPity();

        }
        else
        {
            heroId = RollFeaturedBannerHeroId();
            if (heroId > 0 && IsTierSS(heroId))
                ResetFeaturedPity();
        }
       
        if (heroId <= 0)
        {
            Debug.LogError("Gacha roll failed: heroId <= 0 (check banner pools).");
            return default;
        }

        var result = PlayerInventory.Instance.AddHero(heroId);

        if (result.type == GachaResultType.Shard)
        {
            result.itemId = heroId + 1000;
            result.amount = 10;
        }
        else
        {
            result.amount = 1;
            result.itemId = -1;
        }

        return result;
    }

    public List<GachaResult> RollTen()
    {
        List<GachaResult> results = new List<GachaResult>();
        for (int i = 0; i < 10; i++)
            results.Add(Roll());
        return results;
    }

    int RollStandardBannerHeroId()
    {
        if (standardPityCounter >= standardHardPity && runtime.HasTier(HeroTier.S))
            return runtime.RollHero(HeroTier.S);

        if (standardPityCounter >= standardSoftPityStart && runtime.HasTier(HeroTier.S))
        {
            float p = SoftPityChance(
                standardPityCounter,
                standardSoftPityStart,
                standardHardPity,
                standardSoftPityChanceAtStart,
                standardSoftPityChanceAtEnd);

            if (Random.value < p)
                return runtime.RollHero(HeroTier.S);
        }

        return runtime.RollHero(RollTierNormalCBA());
    }

    int RollFeaturedBannerHeroId()
    {
        // If SS pity triggers, return the selected SS heroId (instead of random SS)
        bool selectedValid = selectedFeaturedHeroId > 0
                             && featuredBanner != null
                             && featuredBanner.featuredPool != null
                             && featuredBanner.featuredPool.Contains(selectedFeaturedHeroId);

        if (featuredPityCounter >= featuredHardPity && runtime.HasTier(HeroTier.SS))
            return selectedValid ? selectedFeaturedHeroId : runtime.RollHero(HeroTier.SS);

        if (featuredPityCounter >= featuredSoftPityStart && runtime.HasTier(HeroTier.SS))
        {
            float p = SoftPityChance(
                featuredPityCounter,
                featuredSoftPityStart,
                featuredHardPity,
                featuredSoftPityChanceAtStart,
                featuredSoftPityChanceAtEnd);

            if (Random.value < p)
                return selectedValid ? selectedFeaturedHeroId : runtime.RollHero(HeroTier.SS);
        }

        // Normal featured outcome: C/B/A/S
        return runtime.RollHero(RollTierNormalCBAS());
    }

    HeroTier RollTierNormalCBA()
    {
        float r = Random.value;
        if (r < 0.70f) return HeroTier.C;
        if (r < 0.95f) return HeroTier.B;
        return HeroTier.A;
    }

    HeroTier RollTierNormalCBAS()
    {
        float r = Random.value;
        if (r < 0.70f) return HeroTier.C;
        if (r < 0.92f) return HeroTier.B;
        if (r < 0.99f) return HeroTier.A;
        return HeroTier.S;
    }

    static float SoftPityChance(int counter, int softStart, int hard, float pStart, float pEnd)
    {
        if (counter < softStart) return 0f;
        if (counter >= hard) return 1f;

        float t = Mathf.InverseLerp(softStart, hard, counter);
        return Mathf.Lerp(pStart, pEnd, t);
    }

    bool IsTierS(int heroId)
    {
        if (activeBanner == null || activeBanner.pool == null) return false;
        for (int i = 0; i < activeBanner.pool.Count; i++)
            if (activeBanner.pool[i].heroId == heroId)
                return activeBanner.pool[i].tier == HeroTier.S;
        return false;
    }

    bool IsTierSS(int heroId)
    {
        if (activeBanner == null || activeBanner.pool == null) return false;
        for (int i = 0; i < activeBanner.pool.Count; i++)
            if (activeBanner.pool[i].heroId == heroId)
                return activeBanner.pool[i].tier == HeroTier.SS;
        return false;
    }

    public List<int> GetFeaturedPool()
    {
        return featuredBanner.featuredPool;
    }
    public BannerTokenExchange GetTokenExchangeList()
    {
        return tokenExchangeList;
    }
}