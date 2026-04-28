using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AbilityType
{
    Normal = 0,
    Skill = 1,
    Ultimate = 2,
    Passive = 3,
    Empower = 4
}

public enum AbilityTargetingMode
{
    // Formation-based
    Row = 0,                 // (1,4) or (2,5) or (3,6)
    Column = 1,              // Front (1,2,3) or Back (4,5,6)
    AoeAllEnemies = 2,       // all enemies
    RandomEnemies = 3,    

    // Scored targets (single)
    HighestAttack = 10,
    LowestAttack = 11,
    HighestHp = 12,
    LowestHp = 13,
    None = 14                
}

public enum ColumnTarget
{
    Front = 1,
    Back = 2
}
public enum PositionAttack
{
    DistanceToTarget = 1,
    MiddlePosition = 2,
    MiddleRow = 3,
    MiddlePositionEnemy = 4,
    CurrentPosition = 5
}
[CreateAssetMenu(menuName = "Hero/Ability Info")]
public class AbilityInfo : ScriptableObject
{
    [Header("Meta")]
    public string abilityName;
    public AbilityType type;

    [Header("Animation State Name")]
    public string animStateName;
    public Sprite icon;

    [Header("Speed run to enemy")]
    public float speedToEnemy;
    [Header("Attack Position")]
    public PositionAttack positionAttack;
    public float distance; // chỉnh khi positionAttack == DistanceToTarget

    [Header("Targeting")]
    public AbilityTargetingMode targetingMode = AbilityTargetingMode.Row;

    [Range(1, 3)]
    public int rowIndex = 1;                 // dùng khi targetingMode == Row

    public ColumnTarget column = ColumnTarget.Front; // dùng khi targetingMode == Column

    [Min(1)]
    public int randomCount = 1;              // dùng khi targetingMode == RandomEnemies

    [Header("Mana (used by caster)")]
    public bool grantManaOnUse = true;
    [Min(0)] public int manaGain;
    public bool isChangeBackGround = false; // có đổi background khi dùng kỹ năng này hay không (áp dụng cho ultimate của hero, không áp dụng cho kỹ năng của đòn đánh thường)
    public Sprite backgroundChange; // background sẽ đổi thành khi dùng 
    public int order; 
    [Header("Effects (executed in order)")]
    public List<AbilityEffect> effects = new List<AbilityEffect>();
    public string description;
    public List<AbilityEffect> GetEffectsStartBattle()
    {
        if (effects == null)
            return null;

        return effects
            .Where(e => e != null && e.timeToCall == TimesToCall.onStartBattle)
            .ToList();
    }
    public List<AbilityEffect> GetEffectsOnAttack()
    {
        if (effects == null)
            return null;

        return effects
            .Where(e => e != null && e.timeToCall == TimesToCall.onAttack)
            .ToList();
    }
    public List<AbilityEffect> GetEffectsOnUse()
    {
        if (effects == null)
            return null;

        return effects
            .Where(e => e != null && e.timeToCall == TimesToCall.OnUse)
            .ToList();
    }
    public List<AbilityEffect> GetEffectsOnSpecial()
    {
        if (effects == null)
            return null;

        return effects
            .Where(e => e != null && e.timeToCall == TimesToCall.Special)
            .ToList();
    }

    public List<AbilityEffect> GetEffectsStartTurn()
    {
        if (effects == null)
            return null;

        return effects
            .Where(e => e != null && e.timeToCall == TimesToCall.onStartTurn)
            .ToList();
    }
}