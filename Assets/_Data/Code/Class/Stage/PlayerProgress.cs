using System.Collections.Generic;

public class PlayerProgress
{
    public int stageId;
    public int currentChapter = 1;
    public int currentStage = 1;
    public List<StageResult> stageResults = new();
}
