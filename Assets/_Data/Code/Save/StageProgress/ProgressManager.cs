using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public PlayerProgress progress;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }
    public int GetStarInChapter(int chapter)
    {
        int totalStar = 0;

        int startId = (chapter - 1) * 10 + 1;
        int endId = chapter * 10;

        foreach (var kvp in progress.stageResult)
        {
            int stageId = kvp.Key;
            int star = kvp.Value;

            if (stageId >= startId && stageId <= endId)
            {
                totalStar += star;
            }
        }

        return totalStar;
    }
    public void UpdateStage(int stageId)
    {
        if (stageId < progress.currentStageId) return;
        progress.currentStageId++;

        progress.currentChapter = (progress.currentStageId - 1) / 10 + 1;
        progress.currentStage = (progress.currentStageId - 1) % 10 + 1;
    }
    public void UpdateStarGain(int star, int stageID)
    {
        if (!progress.stageResult.ContainsKey(stageID))
        {
            progress.stageResult.Add(stageID, star);
            return;
        }

        if (star > progress.stageResult[stageID])
        {
            progress.stageResult[stageID] = star;
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString("player_progress", json);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("player_progress"))
        {
            string json = PlayerPrefs.GetString("player_progress");
            progress = JsonUtility.FromJson<PlayerProgress>(json);
        }
        else
        {
            progress = new PlayerProgress();
        }
    }
    public int  GetChapter()
    {
        return progress.currentChapter;
    }
}
