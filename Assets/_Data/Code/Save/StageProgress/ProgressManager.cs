using UnityEngine;

public class ProgressManager : Subject
{
    public static ProgressManager Instance { get; private set; }

    [SerializeField] public PlayerProgress progress;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(progress == null)
        {
            progress = new PlayerProgress();
        }


    }
    public int GetStarInChapter(int chapter)
    {
        int totalStar = 0;

        int startId = (chapter - 1) * 10 + 1;
        int endId = chapter * 10;

        foreach (var kvp in progress.stageResults)
        {
            int stageId = kvp.stageId;
            int star = kvp.star;

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

        NotifyObservers();
    }
    public void UpdateStarGain(int star, int stageID)
    {
        StageResultData found = progress.stageResults
            .Find(x => x.stageId == stageID);

       
        if (found == null)
        {
            progress.stageResults.Add(new StageResultData
            {
                stageId = stageID,
                star = star
            });
            return;
        }

        
        if (star > found.star)
        {
            found.star = star;
        }
        NotifyObservers();
    }

    //public void Save()
    //{
    //    string json = JsonUtility.ToJson(progress);
    //    PlayerPrefs.SetString("player_progress", json);
    //}

    //public void Load()
    //{
    //    if (PlayerPrefs.HasKey("player_progress"))
    //    {
    //        string json = PlayerPrefs.GetString("player_progress");
    //        progress = JsonUtility.FromJson<PlayerProgress>(json);
    //    }
    //    else
    //    {
    //        progress = new PlayerProgress();
    //    }
    //}
    public int  GetChapter()
    {
        return progress.currentChapter;
    }

    public void SetClaim(int chapterID, int index)
    {
        ChapterRewardClaimedData found =
            progress.chapterRewardsClaimed
            .Find(x => x.chapterId == chapterID);

        if (found == null)
        {
            ChapterRewardClaimedData newData = new ChapterRewardClaimedData();
            newData.chapterId = chapterID;
            newData.rewardsClaimed = new bool[3];

            newData.rewardsClaimed[index] = true;

            progress.chapterRewardsClaimed.Add(newData);
        }
        else
        {
            found.rewardsClaimed[index] = true;
        }

        NotifyObservers();
    }
    public bool IsClaimed(int chapterID, int index)
    {
        ChapterRewardClaimedData found =
            progress.chapterRewardsClaimed
            .Find(x => x.chapterId == chapterID);

        if (found == null)
            return false;

        return found.rewardsClaimed[index];
    }


    public PlayerProgress getProgress()
    {
        return progress;
    }
    public void SetProgress(PlayerProgress playerProgress)
    {
        progress = playerProgress;
    }
}
