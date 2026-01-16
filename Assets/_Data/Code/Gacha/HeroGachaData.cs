public enum HeroTier
{
    D, C, B, S
}

[System.Serializable]
public class HeroGachaData
{
    public int heroId;
    public HeroTier tier;
    //public float rate;
}
