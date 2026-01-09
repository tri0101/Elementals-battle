using System.Net.Mail;
using UnityEngine;

public class PlayerReceiveDamagee : MonoBehaviour, IObserver
{

    [SerializeField] PlayerControl playerControl;
    public PlayerControl PlayerControl => playerControl;


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
        playerControl = transform.parent.parent.GetComponent<PlayerControl>();

       
        maxHealth = playerControl.PlayerInfo.health;
        maxMana = maxHealth * 2;
        physicalArmor = playerControl.PlayerInfo.physicalArmor;
        magicalArmor = playerControl.PlayerInfo.magicArmor;
        mana = 1000000f;
        //mana = 0f;
        health = maxHealth;
        playerControl.RefreshObservers();

    }


    public void ReceiveDamage(Attack attack,PlayerControl otherPlayerControl)
    {
        
        

        if (isDead) return;
        float finalDamage = CalculateDamage(attack, otherPlayerControl);
        if (playerControl.CurrentStringState == "Block" || playerControl.CurrentStringState == "T_Block")
        {
            mana += finalDamage;
            playerControl.RefreshObservers();
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
        playerControl.RefreshObservers();
    }

    public void AddManaWhenAttack(float damage)
    {
        mana += damage * 2;
        if(mana >= 1000)
        {
            mana = 1000f;
        }
        playerControl.RefreshObservers();
    }
    public void CallStopAnim(float duration)
    {
        //isStopAnim = true;
        //stop anim trong vong duration
        DurationFinalAttack = duration;
    }
    public void CallKnockBack(Vector3 knockPosition, float duration)
    {
       
        
        playerControl.PlayerEventt.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, duration);
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

   
    float CalculateDamage(Attack attack, PlayerControl otherPlayerControl)
        {
            switch (attack.AttackInfo.damageType)
            {
                case DamageType.Physical:

                {
                    float damage = attack.AttackDamage;

                    bool isCritical = Random.value < otherPlayerControl.PlayerInfo.criticalRate;

                    if (isCritical)
                    {
                        damage += damage * (otherPlayerControl.PlayerInfo.criticalDamageRate /100);
                        damage *= 2;
                    }

                    return damage * 100 / (100 + physicalArmor); }

                case DamageType.Magical:
                {
                    float damage = attack.AttackDamage;

                    bool isCritical = Random.value < otherPlayerControl.PlayerInfo.criticalRate;

                    if (isCritical)
                    {
                        damage += damage * (otherPlayerControl.PlayerInfo.criticalDamageRate / 100);
                        damage *= 2;
                    }
                    return damage * 100 / (100 + magicalArmor); }
                
                case DamageType.True:
                { return attack.AttackDamage; }
            }

            return attack.AttackDamage;
        }
    


}
