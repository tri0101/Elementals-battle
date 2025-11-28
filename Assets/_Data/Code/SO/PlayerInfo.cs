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
}
