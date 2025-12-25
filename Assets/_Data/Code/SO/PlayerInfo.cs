using UnityEngine;

[CreateAssetMenu(menuName ="Player/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float jumpForce;
    public float airWalkSpeed;
    public float dashPower;
    public float dashDuration;

    [Header("Duration Attack")]
    public float durationA1;
    public float durationA2;
}
