using System.Collections.Generic;
[System.Serializable]
public class StageResultData
{
    public int stageId;
    public int star;
}
[System.Serializable]
public class ChapterRewardClaimedData
{
    public int chapterId;
    public bool[] rewardsClaimed; // Tương ứng với 10/20/30 sao
}
[System.Serializable]
public class PlayerProgress
{
    public int currentStageId = 1;
    public int currentChapter = 1;
    public int currentStage = 1;

    //public Dictionary<int, int> stageResult = new(); // Key: stageId, Value: star
    public List<StageResultData> stageResults = new();

    //// key = chapterID
    //// value = bool[3] tương ứng 10/20/30 sao
    //public Dictionary<int, bool[]> chapterRewardClaimed
    //   = new Dictionary<int, bool[]>();

    public List<ChapterRewardClaimedData> chapterRewardsClaimed = new();


}
