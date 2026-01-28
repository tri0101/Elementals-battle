using System;
using UnityEngine;

public static class FormationManager
{
    const string FormationKey = "player_formation_v1";

    public static void SaveFormation(FormationData formation)
    {
        if (formation == null) return;
        try
        {
            string json = JsonUtility.ToJson(formation);
            PlayerPrefs.SetString(FormationKey, json);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"FormationManager.SaveFormation failed: {e}");
        }
    }

    public static FormationData LoadFormation()
    {
        if (!PlayerPrefs.HasKey(FormationKey))
            return new FormationData();

        try
        {
            string json = PlayerPrefs.GetString(FormationKey);
            var data = JsonUtility.FromJson<FormationData>(json);
            if (data == null || data.slots == null || data.slots.Count != 6)
                return new FormationData();
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"FormationManager.LoadFormation failed: {e}");
            return new FormationData();
        }
    }

    public static int GetHeroAtSlot(FormationData formation, int slotIndex)
    {
        if (formation == null) formation = LoadFormation();
        var s = formation.GetSlot(slotIndex);
        return s != null ? s.heroId : 0;
    }
    
    public static bool AssignHeroToSlot(FormationData formation, int slotIndex, int heroId)
    {
        if (formation == null) formation = LoadFormation();

        if (heroId <= 0)
        {
            formation.SetHero(slotIndex, 0);
            SaveFormation(formation);
            return true;
        }

        foreach (var s in formation.slots)
        {
            if (s == null) continue;
            if (s.slot != slotIndex && s.heroId == heroId)
                return false;
        }

        formation.SetHero(slotIndex, heroId);
        SaveFormation(formation);
        return true;
    }

    public static void RemoveHero(FormationData formation, int slotIndex)
    {
        if (formation == null) formation = LoadFormation();
        formation.RemoveHero(slotIndex);
        SaveFormation(formation);
    }

    public static void ClearFormation()
    {
        SaveFormation(new FormationData());
    }
}