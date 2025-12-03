using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private AttackInfo attackInfo;
    float attackDamage;
    Vector3 knockBack;
    float durationKnock;
    float durationStopping;
    float speed;
    [SerializeField] string myTag;
    
    private void Awake()
    {
        attackDamage = attackInfo.damageSend;
        knockBack = attackInfo.knockBack;
        durationKnock = attackInfo.durationKnockBack;
        durationStopping = attackInfo.durationStopping;
        myTag = transform.parent.parent.parent.tag;
        ;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "ReceiveField")
        {
            

            // Lấy tag đối tượng bị đánh
            string targetTag = other.transform.parent.parent.tag;

            // 1. Nếu target không phải Player hoặc Enemy → bỏ qua luôn
            if (!targetTag.StartsWith("Player") && targetTag != "Enemy")
                return;

            // 2. Nếu là Player → không cho tự đánh chính mình
            if (myTag.StartsWith("Player") && targetTag.StartsWith("Player"))
            {
                if (myTag == targetTag)
                    return; // Player1 không đánh Player1, Player2 không đánh Player2
            }

            // =========================
            //       Enemy bị đánh
            // =========================
            if (targetTag == "Enemy")
            {
                EnemyReceiveDamage enemy = other.GetComponent<EnemyReceiveDamage>();
                if (enemy != null)
                {
                    enemy.ReceiveDamage(attackDamage);
                    enemy.CallKnockBack(knockBack);
                }
                return;
            }

            // =========================
            //       Player bị đánh
            // =========================
           
            PlayerReceiveDamage player = other.GetComponent<PlayerReceiveDamage>();
            if (player != null)
            {
                


                if (player.IsImmortal) return;
                player.ReceiveDamage(attackDamage);
                if (transform.name.Contains("Attack_Final"))
                {
                    player.CallIsFinal();
                }
                if (transform.name.Contains("Skill"))
                {
                    player.ApplyStatus(attackInfo.statusEffect);
                }
                else
                {
                    player.CallStopAnim(durationStopping);
                }

                    

                
                player.CallKnockBack(knockBack, durationKnock);
               

            }



        }
    }
}
