using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/Stage Config")]
public class StageConfig : ScriptableObject
{
    public int stageID;
    public int waveStage;
    [Header("Cost")]
    public int staminaCost;



    [Header("Enemies")]
    public List<EnemySpawnData> enemies;
    [Header("Enemies (by wave)")]
    public List<EnemySpawnEntry> enemySpawns;
    [Header("Drop Items")]
    public List<DropItemData> dropItems;

    public int powerRecommend;
}
