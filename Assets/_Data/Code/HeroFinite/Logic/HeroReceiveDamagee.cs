using System.Net.Mail;
using UnityEngine;

public class HeroReceiveDamagee : MonoBehaviour, IObserver
{

    [SerializeField] HeroControl heroControl;
    public HeroControl HeroControl => heroControl;


    [Header("Attribute")]
    [SerializeField] private float health;
    public float Health
    {
        get => health; set => health = value;
    }
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth; set => maxHealth = value;
    }
    
    [SerializeField] private float mana;
    public float Mana
    {
        get => mana; set => mana = value;
    }
    [SerializeField] private float maxMana;
    public float MaxMana
    {
        get => maxMana; set => maxMana = value;
    }
    [SerializeField] private float physicalArmor;
    public float PhysicalArmor
    {
        get => physicalArmor; set => physicalArmor = value;
    }
    [SerializeField] private float magicalArmor;
    public float MagicalArmor
    {
        get => magicalArmor; set => magicalArmor = value;
    }
    
    [SerializeField] private float durationFinalAttack;

    public float DurationFinalAttack
    {
        get => durationFinalAttack; set => durationFinalAttack = value;
    }

    [Header("Flag")]
    [SerializeField] private bool isHit;
    [SerializeField] private bool isStopAnim = false;
    [SerializeField] private bool isDead = false;
    public bool IsStopAnim
    {
        get => isStopAnim; set => isStopAnim = value;
    }
   
    public bool IsDead
    {
        get => isDead; set => isDead = value;
    }
    public bool IsHit
    {
        get => isHit;
        set => isHit = value;
    }
    private void Awake()
    {
        heroControl = transform.parent.parent.GetComponent<HeroControl>();

       
        maxHealth = heroControl.HeroInfo.health;
        maxMana = maxHealth * 2;
        physicalArmor = heroControl.HeroInfo.armor;
        
        mana = 1000000f;
        //mana = 0f;
        health = maxHealth;
        heroControl.RefreshObservers();

    }


    public void ReceiveDamage(Attack attack, HeroControl otherheroControl)
    {
        
        

        if (isDead) return;
        float finalDamage = CalculateDamage(attack, otherheroControl);
        if (heroControl.CurrentStringState == "Block" || heroControl.CurrentStringState == "T_Block")
        {
            mana += finalDamage;
            heroControl.RefreshObservers();
            return;
        }
        
        health -= finalDamage;

        mana += finalDamage * 1.5f;

        //if(mana >= 1000f)
        //{
        //    mana = 1000f;
        //}
        isHit = true;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
          

        }
        heroControl.RefreshObservers();
    }

    public void AddManaWhenAttack(float damage)
    {
        mana += damage * 2;
        if(mana >= 1000)
        {
            mana = 1000f;
        }
        heroControl.RefreshObservers();
    }
    public void CallStopAnim(float duration)
    {
        //isStopAnim = true;
        //stop anim trong vong duration
        DurationFinalAttack = duration;
    }
    public void CallKnockBack(Vector3 knockPosition, float duration)
    {
       
        
        heroControl.HeroEventt.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, duration);
    }
    public float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }
    public float GetManaPercent()
    {
        return (float)mana / maxMana;
    }
    public void OnNotify()
    {

    }

   
    float CalculateDamage(Attack attack, HeroControl otherheroControl)
        {
            switch (attack.AttackInfo.damageType)
            {
                case DamageType.Physical:

                {
                    float damage = attack.AttackDamage;

                    bool isCritical = Random.value < otherheroControl.HeroInfo.criticalRate;

                    if (isCritical)
                    {
                        damage += damage * (otherheroControl.HeroInfo.criticalDamageRate /100);
                        damage *= 2;
                    }

                    return damage * 100 / (100 + physicalArmor); }

                case DamageType.Magical:
                {
                    float damage = attack.AttackDamage;

                    bool isCritical = Random.value < otherheroControl.HeroInfo.criticalRate;

                    if (isCritical)
                    {
                        damage += damage * (otherheroControl.HeroInfo.criticalDamageRate / 100);
                        damage *= 2;
                    }
                    return damage * 100 / (100 + magicalArmor); }
                
                case DamageType.True:
                { return attack.AttackDamage; }
            }

            return attack.AttackDamage;
        }
    


}
