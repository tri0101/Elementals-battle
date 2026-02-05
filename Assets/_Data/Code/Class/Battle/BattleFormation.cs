using System;
using UnityEngine;

[Serializable]
public class BattleFormation : MonoBehaviour
{
    [Header("Root")]
    public Transform listHeroRoot;

    [Header("Hero Positions (size = 6)")]
    public Transform[] startPositions = new Transform[7];
    public Transform[] battlePositions = new Transform[7];

    [Header("Enemy Root")]
    public Transform listEnemyRoot;

    [Header("Enemy Positions (size = 6)")]
    public Transform[] enemyStartPositions = new Transform[7];
    public Transform[] enemyBattlePositions = new Transform[7];

    public Transform GetStart(int slotIndex1Based)
    {
        int i = slotIndex1Based;
        if (i < 0 || i >= startPositions.Length) return null;
        return startPositions[i];
    }

    public Transform GetBattle(int slotIndex1Based)
    {
        int i = slotIndex1Based;
        if (i < 0 || i >= battlePositions.Length) return null;
        return battlePositions[i];
    }

    public Transform GetEnemyStart(int slotIndex1Based)
    {
        int i = slotIndex1Based;
        if (i < 0 || i >= enemyStartPositions.Length) return null;
        return enemyStartPositions[i];
    }

    public Transform GetEnemyBattle(int slotIndex1Based)
    {
        int i = slotIndex1Based;
        if (i < 0 || i >= enemyBattlePositions.Length) return null;
        return enemyBattlePositions[i];
    }
}