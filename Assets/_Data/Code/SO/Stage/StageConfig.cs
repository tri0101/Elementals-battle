using System.Collections.Generic;
using UnityEngine;
public enum StageCondition
{
    HeroLost = 0

}
[CreateAssetMenu(menuName = "Stage/Stage Config")]
public class StageConfig : ScriptableObject
{
    public int stageID;
    public int waveStage;
    [Header("Cost")]
    public int staminaCost;
    public int expForAliveHero;
    public int expForPlayer;
    [Header("Stage Condition")]// chỉ dành cho sao 2 , sao 1 và 3 cố định
    public StageCondition stageCondition;
    public int conditionParam;// ví dụ hero mất bao nhiêu người ( dành cho heroLost)

    [Header("Enemies")]
    public List<EnemySpawnData> enemies;
    [Header("Enemies (by wave)")]
    public List<EnemySpawnEntry> enemySpawns;
    [Header("Drop Items")]
    public List<DropItemData> dropItems;

    public int powerRecommend;
}
