using UnityEngine;
public static class HeroRankHelper
{
    private static int blackRank = 1;
    private static int greenRank = 5;

    private static Color blackColor = new Color(157 / 255f, 143 / 255f, 143 / 255f);
    private static Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    public static void GetRankVisual(
        int rank,
        string heroName,
        out string displayName,
        out Color nameColor
    )
    {
        int plusValue = 0;

        if (rank < greenRank)
        {
            nameColor = Color.white;
            if (rank > blackRank)
                plusValue = rank - blackRank;
        }
        else
        {
            nameColor = greenColor;
            if (rank > greenRank)
                plusValue = rank - greenRank;
        }

        displayName = plusValue > 0
            ? $"{heroName} +{plusValue}"
            : heroName;
    }
}
