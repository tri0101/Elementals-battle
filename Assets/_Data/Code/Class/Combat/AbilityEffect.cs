using System;
using UnityEngine;

public enum AbilityEffectType
{
    Damage = 0,
    Heal = 1,
    AddMana = 2,

    // Buff/Debuff generic (tăng/giảm stat)
    ModifyStat = 10,

    // CC
    Stun = 20,
    Paralyze = 21
}

public enum AbilityTarget
{
    CurrentTarget = 0,   // mục tiêu đang đánh
    Self = 1,
    AllyLowestHp = 2,
    AllyAll = 3,
    EnemyAll = 4
}

public enum StatType
{
    Attack = 0,
    Defense = 1,
    Speed = 2
}

[Serializable]
public class AbilityEffect
{
    public AbilityEffectType type;

    [Header("Targeting")]
    public AbilityTarget target = AbilityTarget.CurrentTarget;

    [Header("Proc")]
    [Range(0f, 1f)] public float chance = 1f;

    [Header("Value")]
    // Ý nghĩa tùy effect:
    // - Damage: multiplier hoặc flat (tùy bạn unify sau)
    // - Heal: % maxHP hoặc flat
    // - ModifyStat: % hoặc flat
    public float value = 0f;

    [Header("Duration (seconds or turns)")]
    public float duration = 0f;

    [Header("Stat (for ModifyStat)")]
    //chỉ dùng khi type == ModifyStat
    public StatType statType = StatType.Attack;

    // -1 = debuff, +1 = buff (hoặc bạn có thể dùng value âm dương)
    public int direction = -1;
}