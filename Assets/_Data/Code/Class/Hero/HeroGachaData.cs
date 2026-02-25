using UnityEngine;

public enum HeroTier
{
    D = 0,
    C = 1,
    B = 2,
    S = 3
}

[System.Serializable]
public class HeroGachaData
{
    public int heroId;
    public HeroTier tier;
    //public float rate;
}

[System.Serializable]
public class ItemGachaData
{
    public int itemId;
    [Min(1)] public int amount = 1;

    [Range(0f, 1f)]
    public float weight = 1f; 
}