using System.Collections.Generic;
using UnityEngine;

public enum GachaBannerType
{
    Standard = 0,
    Featured = 1
}

[CreateAssetMenu(menuName = "Gacha/Banner")]
public class GachaBanner : ScriptableObject
{
    [Header("Base Pools")]
    public List<HeroGachaData> pool;
    public List<ItemGachaData> itemPool;

    [Header("Banner Type")]
    public GachaBannerType bannerType = GachaBannerType.Standard;

    [Header("Featured Banner (Choose 1 of 4 SS)")]
    [Tooltip("Exactly 4 heroIds (tier SS). Player chooses 1 of them.")]
    public List<int> featuredPool = new();

    [Header("Featured SS Pity (soft 80-120, hard 120)")]
    [Min(1)] public int featuredSoftPityStart = 80;
    [Min(1)] public int featuredHardPity = 120;
}