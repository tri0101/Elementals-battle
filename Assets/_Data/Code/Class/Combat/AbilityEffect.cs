using System;
using UnityEngine;

public enum AbilityEffectType
{

    
    ModifyStat = 0,         // Buff/Debuff generic (tăng/giảm stat)
    Burn = 1,                 // thiêu đốt
    // CC
    Rooted = 19,            // trói chân
    Stun = 20,              // choáng
    Paralyze = 21           // tê liệt
}

public enum AbilityTarget
{
    CurrentTarget = 0,      // mục tiêu đang đánh
    Self = 1,               //bản thân
    HeroLowestHp = 2,       // đồng minh có HP thấp nhất
    HeroHighestDamage = 3,  // đồng minh có sát thương cao nhất
    EnemyAll = 4,           // tất cả kẻ địch
    HeroAll = 5,            // tất cả đồng minh
    DPSEnemyAll = 6,        // tất cả kẻ địch có hệ DPS
    TankEnemyAll = 7,       // tất cả kẻ địch có hệ Tank
    SupportEnemyAll = 8,    // tất cả kẻ địch có hệ Support
    DPSHeroAll = 9,         // tất cả đồng minh có hệ DPS
    TankHeroAll = 10,       // tất cả đồng minh có hệ Tank
    SupportHeroAll = 11,    // tất cả đồng minh có hệ Support
    FrontHeroColumns = 12,     // cột đồng minh phía trước
    BackHeroColumns = 13,      // cột đồng minh phía sau
    HeroRow = 14,              // hàng đồng minh cùng row với hero
}

public enum ModifyStatType
{
    Damage = 0,       //tăng damage
    Health = 1,           //tăng hp
    Speed = 2,       // tăng speed
    ArmorIncreased= 3,       // tăng armor
    ArmorDecreased = 4,       // giảm armor
    CritRate = 5,    // tăng tỉ lệ chí mạng
    CritDamage = 6,   // tăng tỉ lệ dame chí mạng
}
public enum TimesToCall
{
    onStartBattle = 0,   //gọi khi bắt đầu trận đấu
    onStartTurn = 1,    //gọi khi bắt đầu lượt của hero
    onAttack = 2,       //gọi khi hero tấn công
    onAttacked = 3,     //gọi khi hero bị tấn công
}
[Serializable]
public class AbilityEffect
{
    public AbilityEffectType type;

    [Header("Trigger")]
    public TimesToCall timeToCall = TimesToCall.onAttack;
    [Header("Targeting")]
    public AbilityTarget target = AbilityTarget.CurrentTarget;

    [Header("Proc")]
    [Range(0f, 1f)] public float chance = 1f; // tỉ lệ ra hiệu ứng (0-1)


    //nếu stat = modify thì value là % tăng type đó
    //nếu stat = burn thì value là % sát thương so với type
    [Header("Stat")]
    public ModifyStatType statType = ModifyStatType.Damage;
    [Header("Value")]
    public bool canUpgrade = false; // có thể nâng cấp hiệu ứng này khi nâng cấp kỹ năng hay không
    public float modifyValue = 0f; // giá trị tăng ( %)
    public int stackCount = 1; // số stack của hiệu ứng ;
    [Header("Damage over Time( for burn")]

    [Header("Duration")]
    public int durationTurn = 1 ; // thời gian hiệu lực khống chế /buff/debuff (turn) , bằng - 1 = vĩnh viễn;
   
}