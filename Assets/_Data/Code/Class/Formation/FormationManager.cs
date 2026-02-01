using UnityEngine;

public static class FormationManager
{
    private const string KEY = "PLAYER_FORMATION_V1";

    public static void Save(int[] heroIds)
    {
        FormationData data = new FormationData { heroIds = heroIds };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static int[] Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return new FormationData().heroIds;

        string json = PlayerPrefs.GetString(KEY);
        FormationData data = JsonUtility.FromJson<FormationData>(json);
        return data.heroIds ?? new FormationData().heroIds;
    }
}