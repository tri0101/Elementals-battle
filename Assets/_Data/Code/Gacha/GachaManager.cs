using UnityEngine;
using System.Collections.Generic;

public class GachaManager : MonoBehaviour, IGachaService
{
    public static GachaManager Instance { get; private set; }

    public GachaBanner banner;
    private GachaBannerRuntime runtime;

    public int pullCount;
    private const int PITY_S = 120;

    [SerializeField] private int oneDiamondCost = 220;
    [SerializeField] private int tenDiamondCost = 2580;
    [SerializeField] private int oneTicketCost = 1;
    [SerializeField] private int tenTicketCost = 10;

    [Header("Item Drop Rate")]
    [Range(0f, 1f)]
    [SerializeField] private float itemDropChance = 0.8f; // 10% ra item (nếu itemPool có)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (runtime == null)
            runtime = new GachaBannerRuntime(banner);
    }

    public GachaResult Roll()
    {
        if (PlayerInventory.Instance)
            pullCount++;

        // 1) Nếu banner có itemPool và random trúng item => trả item
        if (banner != null && banner.itemPool != null && banner.itemPool.Count > 0)
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

        // 2) Roll hero như cũ (có pity S)
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

        // AddHero hiện tại trả Hero hoặc Shard (và shard có AddItem(heroId+1000,10))
        var result = PlayerInventory.Instance.AddHero(heroId);

        // patch thêm amount/itemId nếu là shard để UI có thể lấy ItemData luôn
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

    HeroTier RollTierNormal()
    {
        float rand = Random.value;

        if (rand < 0.6f) return HeroTier.D;
        if (rand < 0.9f) return HeroTier.C;
        return HeroTier.B;
    }
}