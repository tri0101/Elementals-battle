using System.Collections.Generic;

public class PlayerProgress
{
    public int currentStageId = 1;
    public int currentChapter = 1;
    public int currentStage = 1;
    //public List<StageResult> stageResults = new();
    public Dictionary<int, int> stageResult = new(); // Key: stageId, Value: star
}
