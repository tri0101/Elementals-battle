using UnityEngine;
public static class HeroRankHelper
{
    private static int blackRank = 1;
    private static int greenRank = 5;
    private static int blueRank = 9;
    private static int yellowRank = 13;
    private static Color blackColor = new Color(157 / 255f, 143 / 255f, 143 / 255f);
    private static Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    private static Color blueColor = new Color(0f, 38f / 255f, 1f);
    private static Color yellowColor = new Color(1f, 1f, 0f);

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
            // giữ behavior cũ
            nameColor = Color.white;

            if (rank > blackRank)
                plusValue = rank - blackRank;
        }
        else if (rank < blueRank)
        {
            nameColor = greenColor;

            if (rank > greenRank)
                plusValue = rank - greenRank;
        }
        else if(rank < yellowRank)
        {
            nameColor = blueColor;

            if (rank > blueRank)
                plusValue = rank - blueRank;
        }
        else
        {
            nameColor = yellowColor;
            if (rank > yellowRank)
                plusValue = rank - yellowRank;
        }

        displayName = plusValue > 0
            ? $"{heroName} +{plusValue}"
            : heroName;
    }
}