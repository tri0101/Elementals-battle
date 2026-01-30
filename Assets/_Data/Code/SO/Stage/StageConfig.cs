using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/Stage Config")]
public class StageConfig : ScriptableObject
{
    //public int chapter;
    //public int stage;
    public int stageID;
    [Header("Cost")]
    public int staminaCost;
    [Header("Enemies")]
    public List<EnemySpawnData> enemies;

    [Header("Drop Items")]
    public List<DropItemData> dropItems;

    public int powerRecommend;
}
