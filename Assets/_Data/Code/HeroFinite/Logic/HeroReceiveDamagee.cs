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

       
        //maxHealth = heroControl.HeroInfo.health;
        //maxMana = 1000f;
        //physicalArmor = heroControl.HeroInfo.armor;

        //mana = 0f;
        ////mana = 0f;
        //health = maxHealth;
        //heroControl.RefreshObservers();

    }


    public void ReceiveDamage(float damage)
    {


        heroControl.SetIsTakeHit();
        heroControl.HeroStatRuntime.MinusHP((int)damage);
        heroControl.HeroStatRuntime.GainMana(200);
        if(heroControl.HeroStatRuntime.CurrentHealth <= 0)
        {
            isDead = true;
           
            heroControl.SetIsDead();
        }
    }


    public void CallStopAnim(float duration)
    {
        //isStopAnim = true;
        //stop anim trong vong duration
        DurationFinalAttack = duration;
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

   
   


}
