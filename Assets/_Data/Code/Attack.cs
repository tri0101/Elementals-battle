using UnityEngine;

public class Attack : Subject
{
    [SerializeField] private AttackInfo attackInfo;
    public AttackInfo AttackInfo => attackInfo;
    [SerializeField] private HeroControl heroControl;
    [SerializeField] private float attackDamage;
    public float AttackDamage => attackDamage;
    Vector3 knockBack;
    float durationKnock;
    float durationStopping;
    float speed;
    [SerializeField] string myTag;
    
    private void Awake()
    {
        
        knockBack = attackInfo.knockBack;
        durationKnock = attackInfo.durationKnockBack;
        durationStopping = attackInfo.durationStopping;

        myTag = transform.parent.parent.parent.tag;
        heroControl = transform.parent.parent.parent.GetComponent<HeroControl>();
        //SetDamage();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "ReceiveField") return;

        // Tag của kẻ bị đánh
        string targetTag = other.transform.parent.parent.tag;

        // 1. Bỏ qua nếu không phải hero hoặc Enemy
        if (!targetTag.StartsWith("Player") && targetTag != "Enemy")
            return;

        // 2. Nếu là tấn công gần hoặc ranged → không cho tự đánh bản thân
        // myTag = tag của nguồn tấn công (chính bạn)
        // rangedTag = nếu là đạn bắn ra

        string rangedTag = transform.parent.parent.tag; // ví dụ: RangedAttackhero1 hoặc RangedAttackhero2

        bool isRangedFromP1 = rangedTag == "RangedAttackPlayer1";
        bool isRangedFromP2 = rangedTag == "RangedAttackPlayer2";

        bool attackerIsP1 = myTag == "Player1" || isRangedFromP1;
        bool attackerIsP2 = myTag == "Player2" || isRangedFromP2;

        bool targetIsP1 = targetTag == "Player1";
        bool targetIsP2 = targetTag == "Player2";

        // Không cho hero1 đánh hero1
        if (attackerIsP1 && targetIsP1) return;

        // Không cho hero2 đánh hero2
        if (attackerIsP2 && targetIsP2) return;


        // =========================
        //       Enemy bị đánh
        // =========================
        if (targetTag == "Enemy")
        {
            ////var enemy = other.GetComponent<EnemyReceiveDamage>();
            //if (enemy != null)
            //{
            //    enemy.ReceiveDamage(attackDamage);
            //    enemy.CallKnockBack(knockBack);
            //}

          
            return;
        }


        // =========================
        //       hero bị đánh
        // =========================
        var hero = other.GetComponent<HeroReceiveDamagee>();
        if (hero != null)
        {
            //if (hero.IsImmortal) return;

            hero.ReceiveDamage(this ,heroControl);

            //if (transform.name.Contains("Attack_Final"))
            //    hero.CallIsFinal();

            //if (attackInfo.statusEffect != StatusEffect.Normal)
            //    hero.ApplyStatus(attackInfo.statusEffect);
            //else
            hero.CallStopAnim(durationStopping);

            hero.CallKnockBack(knockBack, durationKnock);
            NotifyObservers(this);
        }
    }

    void SetDamage()
    {
       
            attackDamage = attackInfo.damageSend + heroControl.HeroInfo.damage;
        
       

    }
}
