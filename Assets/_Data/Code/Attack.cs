using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private AttackInfo attackInfo;
    float attackDamage;
    Vector3 knockBack;
    private void Awake()
    {
        attackDamage = attackInfo.damageSend;
        knockBack = attackInfo.knockBack;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "ReceiveField")
        {
            if(other.transform.parent.parent.tag == "Enemy")
            {
                EnemyReceiveDamage enemyReceiveDamage = other.gameObject.GetComponent<EnemyReceiveDamage>();
                enemyReceiveDamage.ReceiveDamage(attackDamage);
                enemyReceiveDamage.CallKnockBack(knockBack);
            }
            else
            {
                PlayerReceiveDamage playerReceiveDamage = other.gameObject.GetComponent<PlayerReceiveDamage>();
                playerReceiveDamage.ReceiveDamage(attackDamage);
                playerReceiveDamage.CallKnockBack(knockBack);
            }
        }
    }
}
