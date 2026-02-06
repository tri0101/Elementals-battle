using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Normal = 0,
    Skill = 1,
    Ultimate = 2,
    Passive = 3
}

public enum AbilityTargetingMode
{
    // Formation-based
    Row = 0,                 // (1,4) or (2,5) or (3,6)
    Column = 1,              // Front (1,2,3) or Back (4,5,6)
    AoeAllEnemies = 2,       // all enemies
    RandomEnemies = 3,       // random N enemies

    // Scored targets (single)
    HighestAttack = 10,
    LowestAttack = 11,
    HighestHp = 12,
    LowestHp = 13,
    None = 14               // normal 
}

public enum ColumnTarget
{
    Front = 1,
    Back = 2
}

[CreateAssetMenu(menuName = "Hero/Ability Info")]
public class AbilityInfo : ScriptableObject
{
    [Header("Meta")]
    public string abilityName;
    public AbilityType type;

    [Header("Animation State Name")]
    public string animStateName;

    [Header("Targeting")]
    public AbilityTargetingMode targetingMode = AbilityTargetingMode.Row;

    [Range(1, 3)]
    public int rowIndex = 1;                 // dùng khi targetingMode == Row

    public ColumnTarget column = ColumnTarget.Front; // dùng khi targetingMode == Column

    [Min(1)]
    public int randomCount = 1;              // dùng khi targetingMode == RandomEnemies

    [Header("Mana (used by caster)")]
    public bool grantManaOnUse = true;
    [Min(0)] public int manaGain = 10;

    [Header("Effects (executed in order)")]
    public List<AbilityEffect> effects = new List<AbilityEffect>();
}