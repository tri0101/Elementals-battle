public static class StageResultHelper
{
    //public static StageResult GetOrCreate(int stageId)
    //{
    //    var progress = ProgressManager.Instance.progress;

    //    StageResult result =
    //        progress.stageResults.Find(s => s.stageId == stageId);

    //    if (result == null)
    //    {
    //        result = new StageResult
    //        {
    //            stageId = stageId,
    //            star = 0,
    //            cleared = false
    //        };
    //        progress.stageResults.Add(result);
    //    }

    //    return result;
    //}
    //public static int GetStageStar(int stageId)
    //{
    //    var result = ProgressManager.Instance.progress.stageResults
    //        .Find(s => s.stageId == stageId);

    //    return result != null ? result.star : 0;
    //}
    //public static bool IsStageUnlocked(int stageId)
    //{
    //    int prevStageId = stageId - 1;

    //    if (prevStageId % 100 == 0) return true; // stage đầu chapter

    //    var prev = ProgressManager.Instance.progress.stageResults
    //        .Find(s => s.stageId == prevStageId);

    //    return prev != null && prev.cleared;
    //}


  

}
