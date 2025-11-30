using UnityEngine;

[CreateAssetMenu(menuName = "Player/AttackInfo")]
public class AttackInfo : ScriptableObject
{
    public float damageSend;
    public Vector3 knockBack;
    public float durationKnockBack;
    public float durationStopping;
    public float speed;
}
