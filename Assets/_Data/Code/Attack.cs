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
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "ReceiveField")
        {
            //if (myTag.StartsWith("Player"))
            //{
            //    if (other.transform.parent.parent.tag == "Enemy")
            //    {
            //        EnemyReceiveDamage enemyReceiveDamage = other.gameObject.GetComponent<EnemyReceiveDamage>();
            //        enemyReceiveDamage.ReceiveDamage(attackDamage);
            //        enemyReceiveDamage.CallKnockBack(knockBack);
            //    }
            //    else if(myTag == "Player1")
            //    {
            //        if(other.transform.parent.parent.tag == "Player2")
            //        {
            //            PlayerReceiveDamage playerReceiveDamage = other.gameObject.GetComponent<PlayerReceiveDamage>();
            //            playerReceiveDamage.ReceiveDamage(attackDamage);
            //            playerReceiveDamage.CallKnockBack(knockBack);
            //        }

            //    }
            //    else if (myTag == "Player2")
            //    {
            //        if (other.transform.parent.parent.tag == "Player1")
            //        {
            //            PlayerReceiveDamage playerReceiveDamage = other.gameObject.GetComponent<PlayerReceiveDamage>();
            //            playerReceiveDamage.ReceiveDamage(attackDamage);
            //            playerReceiveDamage.CallKnockBack(knockBack);
            //        }

            //    }

            //}
            //else
            //{

            //    if (other.transform.parent.parent.tag.StartsWith("Player"))
            //    {
            //        PlayerReceiveDamage playerReceiveDamage = other.gameObject.GetComponent<PlayerReceiveDamage>();
            //        playerReceiveDamage.ReceiveDamage(attackDamage);
            //        playerReceiveDamage.CallKnockBack(knockBack);
            //    }


            //}



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
                //player.ReceiveDamage(attackDamage);
                //if (transform.name.Contains("Attack_4"))
                //{
                //    player.CallKnockBackBySpeed(knockBack, speed);

                //}
                //else
                //{
                //    player.CallKnockBack(knockBack, duration);
                //}
                if (transform.name.Contains("Attack_Final"))
                {
                    player.CallIsFinal();
                }
                player.CallStopAnim(durationStopping);

                player.ReceiveDamage(attackDamage);
                player.CallKnockBack(knockBack, durationKnock);
                
            }



        }
    }
}
