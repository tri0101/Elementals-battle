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
