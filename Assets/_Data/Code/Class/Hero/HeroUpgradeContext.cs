public static class HeroUpgradeContext
{
    public enum OpenMode
    {
        Upgrade,
        Preview
    }

    public static HeroViewData SelectedHero;
    public static OpenMode Mode = OpenMode.Upgrade;
}