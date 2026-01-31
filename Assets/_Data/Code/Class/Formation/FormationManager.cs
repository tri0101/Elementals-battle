using UnityEngine;

public static class FormationManager
{
    const string FormationKey = "player_formation_v1";

    public static FormationData LoadFormation()
    {
        if (!PlayerPrefs.HasKey(FormationKey))
            return new FormationData();

        string json = PlayerPrefs.GetString(FormationKey);
        return JsonUtility.FromJson<FormationData>(json);
    }

    public static void SaveFormation(FormationData formation)
    {
        string json = JsonUtility.ToJson(formation);
        PlayerPrefs.SetString(FormationKey, json);
    }

    public static int GetHeroAtSlot(FormationData formation, int slot)
    {
        return formation.GetHero(slot);
    }

    public static bool AssignHeroToSlot(FormationData formation, int slot, int heroId)
    {
        // Không cho trùng hero
        foreach (var kv in formation.slots)
        {
            if (kv.Value == heroId)
                return false;
        }

        formation.SetHero(slot, heroId);
        SaveFormation(formation);
        return true;
    }

    public static int FindFirstEmptySlot(FormationData formation, int maxSlot = 6)
    {
        for (int i = 1; i <= maxSlot; i++)
        {
            if (formation.GetHero(i) <= 0)
                return i;
        }
        return -1; // full
    }

    public static void RemoveHero(FormationData formation, int slot)
    {
        formation.RemoveHero(slot);
        SaveFormation(formation);
    }
}
