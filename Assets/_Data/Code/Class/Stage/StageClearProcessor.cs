public class StageClearProcessor
{
    public static void OnStageClear(int stageId, int starEarned)
    {
        var result = StageResultHelper.GetOrCreate(stageId);

        result.cleared = true;

        // chỉ cập nhật nếu sao cao hơn
        if (starEarned > result.star)
        {
            result.star = starEarned;
        }

        //ProgressManager.Instance.Save();
    }
}
