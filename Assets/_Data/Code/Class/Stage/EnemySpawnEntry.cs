using System;
using UnityEngine;

[Serializable]
public class EnemySpawnEntry
{
    [Min(1)] public int wave = 1;

    
    [Range(1, 6)] public int slotIndex = 1;

    public int heroId;
}