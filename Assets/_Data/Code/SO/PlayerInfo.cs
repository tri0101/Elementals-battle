using UnityEngine;

[CreateAssetMenu(menuName ="Player/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float jumpForce;
    public float airWalkSpeed;
    public float rollPower;
    public float rollDuration;

    [Header("Duration Attack")]
    public float durationA1;
    public float durationA2;
    [Header("Duration SKill")]
    public float durationSkill;
    public float durationRangedAttack;
    [Header("Duration Transform SKill")]
    public float durationTransformSkill1;
    public float durationTransformSkill2;
    public float durationTransformSkill3;
}
