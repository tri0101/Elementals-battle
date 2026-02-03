using System;
using UnityEngine;

[Serializable]
public class BattleFormation : MonoBehaviour
{
    [Header("Root")]
    public Transform listHeroRoot;

    [Header("Positions (size = 6)")]
    public Transform[] startPositions = new Transform[7];
    public Transform[] battlePositions = new Transform[7];

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
}