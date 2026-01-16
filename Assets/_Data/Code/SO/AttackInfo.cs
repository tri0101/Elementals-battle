using UnityEngine;

[CreateAssetMenu(menuName = "hero/AttackInfo")]
public class AttackInfo : ScriptableObject
{

    public DamageType damageType;

    public float damageSend;
    public Vector3 knockBack;

   
    public float durationKnockBack;

    //Don attack se stop anim cua doi thu bao nhieu giay
    public float durationStopping;


    public float speed;
    public StatusEffect statusEffect;
   
}
