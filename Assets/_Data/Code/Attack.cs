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



        string rangedTag = transform.parent.parent.tag; 

        bool isRangedFromP1 = rangedTag == "RangedAttackPlayer1";
        bool isRangedFromP2 = rangedTag == "RangedAttackPlayer2";

        bool attackerIsP1 = myTag == "Player1" || isRangedFromP1;
        bool attackerIsP2 = myTag == "Player2" || isRangedFromP2;

        bool targetIsP1 = targetTag == "Player1";
        bool targetIsP2 = targetTag == "Player2";

       


        // =========================
        //       hero bị đánh
        // =========================
        var hero = other.GetComponent<HeroReceiveDamagee>();
        if (hero != null)
        {
           

            hero.ReceiveDamage(this ,heroControl);

           


            NotifyObservers(this);
        }
    }

    void SetDamage()
    {
       
            attackDamage = attackInfo.damageSend + heroControl.HeroInfo.damage;
        
       

    }
}
