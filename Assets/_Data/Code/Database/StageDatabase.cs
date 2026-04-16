using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/Stage Database")]
public class StageDatabase : ScriptableObject
{
    public List<StageConfig> stages;

    private Dictionary<int, StageConfig> stageDict;
    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        stageDict = new Dictionary<int, StageConfig>();

        foreach (var stage in stages)
        {
            stageDict[stage.stageID] = stage;
        }
    }

    public StageConfig GetStage(int stageId)
    {
       if (stageDict == null) Init();

        return stageDict.TryGetValue(stageId, out var stage)
            ? stage
            : null;
    }
}
