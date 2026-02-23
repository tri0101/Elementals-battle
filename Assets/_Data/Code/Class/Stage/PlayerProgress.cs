using System.Collections.Generic;

public class PlayerProgress
{
    public int currentStageId = 1;
    public int currentChapter = 1;
    public int currentStage = 1;

    public Dictionary<int, int> stageResult = new(); // Key: stageId, Value: star


    // key = chapterID
    // value = bool[3] tương ứng 10/20/30 sao
    public Dictionary<int, bool[]> chapterRewardClaimed
       = new Dictionary<int, bool[]>();
}
