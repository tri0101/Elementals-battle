using UnityEngine;

[CreateAssetMenu(menuName ="Player/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    [Header("Attribute Gereral")]
    public float physicalDamage;
    public float magicalDamage;

    public float health;
    public float moveSpeed;
    public float jumpForce;
    public float airWalkSpeed;
    public float rollPower;
    public float rollDuration;
    [Header("Attribute Defent")]
    public float magicArmor;
    public float physicalArmor;
    [Header("Attribute probability")]
    public float criticalRate; // tỉ lệ chí mạng
    public float criticalDamageRate; // sát thương chí mạng

    [Header("Attribute Cooldown Reduction")]
    public float skill_one_CDR;
    public float skill_two_CDR;
    public float skill_three_CDR;




    [Header("Duration Attack")]
    public float durationA1;
    public float durationA2;


    [Header("Duration SKill")]
    
    public float durationSkillOne;
    public float durationRangedAttack;
    public float durationSkill;
    [Header("Duration Transform SKill")]
    public float durationTransformSkill1;
    public float durationTransformSkill2;
    public float durationTransformSkill3;
}
