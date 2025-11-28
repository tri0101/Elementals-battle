using UnityEngine;

[CreateAssetMenu(menuName = "Player/AttackInfo")]
public class AttackInfo : ScriptableObject
{
    public float damageSend;
    public Vector3 knockBack;
    public float duration;
    public float speed;
}
