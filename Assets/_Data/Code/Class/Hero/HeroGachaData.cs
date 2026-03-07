using UnityEngine;

public enum HeroTier
{
    C = 0,
    B = 1,
    A = 2,
    S = 3,
    SS = 4
}

[System.Serializable]
public class HeroGachaData
{
    public int heroId;
    public HeroTier tier;
}

[System.Serializable]
public class ItemGachaData
{
    public int itemId;
    [Min(1)] public int amount = 1;

    [Range(0f, 1f)]
    public float weight = 1f;
}