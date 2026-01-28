using UnityEngine;

public static class FormationContext
{
    public static int SelectedSlotIndex = 0;
    public static HeroViewData SelectedHero = null;

    public static void Clear()
    {
        SelectedSlotIndex = 0;
        SelectedHero = null;
    }
}