using System;
using UnityEngine;

[Serializable]
public class BattleFormation : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private Transform listHeroRoot;
    public Transform ListHeroRoot => listHeroRoot;

    [Header("Hero Positions (size = 6)")]
    [SerializeField] private Transform[] startPositions = new Transform[7];
    [SerializeField] private Transform[] battlePositions = new Transform[7];

    [Header("Enemy Root")]
    [SerializeField] private Transform listEnemyRoot;
    public Transform ListEnemyRoot => listEnemyRoot;

    [Header("Enemy Positions (size = 6)")]
    [SerializeField] private Transform[] enemyStartPositions = new Transform[7];
    [SerializeField] private Transform[] enemyBattlePositions = new Transform[7];

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