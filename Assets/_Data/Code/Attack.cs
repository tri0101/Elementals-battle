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
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(other.tag == "ReceiveField")
    //    {


    //        // Lấy tag đối tượng bị đánh
    //        string targetTag = other.transform.parent.parent.tag;

    //        // 1. Nếu target không phải Player hoặc Enemy → bỏ qua luôn
    //        if (!targetTag.StartsWith("Player") && targetTag != "Enemy")
    //            return;

    //        // 2. Nếu là Player → không cho tự đánh chính mình
    //        if (myTag.StartsWith("Player") && targetTag.StartsWith("Player"))
    //        {
    //            if (myTag == targetTag)
    //                return; // Player1 không đánh Player1, Player2 không đánh Player2
    //        }

    //        // =========================
    //        //       Enemy bị đánh
    //        // =========================
    //        if (targetTag == "Enemy")
    //        {
    //            EnemyReceiveDamage enemy = other.GetComponent<EnemyReceiveDamage>();
    //            if (enemy != null)
    //            {
    //                enemy.ReceiveDamage(attackDamage);
    //                enemy.CallKnockBack(knockBack);
    //            }
    //            return;
    //        }

    //        // =========================
    //        //       Player bị đánh
    //        // =========================

    //        PlayerReceiveDamage player = other.GetComponent<PlayerReceiveDamage>();
    //        if (player != null)
    //        {



    //            if (player.IsImmortal) return;
    //            player.ReceiveDamage(attackDamage);
    //            if (transform.name.Contains("Attack_Final"))
    //            {
    //                player.CallIsFinal();
    //            }
    //            if (attackInfo.statusEffect != StatusEffect.Normal)
    //            {
    //                player.ApplyStatus(attackInfo.statusEffect);
    //            }
    //            else
    //            {
    //                player.CallStopAnim(durationStopping);
    //            }




    //            player.CallKnockBack(knockBack, durationKnock);


    //        }

    //    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "ReceiveField") return;

        // Tag của kẻ bị đánh
        string targetTag = other.transform.parent.parent.tag;

        // 1. Bỏ qua nếu không phải Player hoặc Enemy
        if (!targetTag.StartsWith("Player") && targetTag != "Enemy")
            return;

        // 2. Nếu là tấn công gần hoặc ranged → không cho tự đánh bản thân
        // myTag = tag của nguồn tấn công (chính bạn)
        // rangedTag = nếu là đạn bắn ra

        string rangedTag = transform.parent.parent.tag; // ví dụ: RangedAttackPlayer1 hoặc RangedAttackPlayer2

        bool isRangedFromP1 = rangedTag == "RangedAttackPlayer1";
        bool isRangedFromP2 = rangedTag == "RangedAttackPlayer2";

        bool attackerIsP1 = myTag == "Player1" || isRangedFromP1;
        bool attackerIsP2 = myTag == "Player2" || isRangedFromP2;

        bool targetIsP1 = targetTag == "Player1";
        bool targetIsP2 = targetTag == "Player2";

        // Không cho Player1 đánh Player1
        if (attackerIsP1 && targetIsP1) return;

        // Không cho Player2 đánh Player2
        if (attackerIsP2 && targetIsP2) return;


        // =========================
        //       Enemy bị đánh
        // =========================
        if (targetTag == "Enemy")
        {
            var enemy = other.GetComponent<EnemyReceiveDamage>();
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
        var player = other.GetComponent<PlayerReceiveDamage>();
        if (player != null)
        {
            if (player.IsImmortal) return;

            player.ReceiveDamage(attackDamage);

            if (transform.name.Contains("Attack_Final"))
                player.CallIsFinal();

            if (attackInfo.statusEffect != StatusEffect.Normal)
                player.ApplyStatus(attackInfo.statusEffect);
            else
                player.CallStopAnim(durationStopping);

            player.CallKnockBack(knockBack, durationKnock);

        }
    }


}
