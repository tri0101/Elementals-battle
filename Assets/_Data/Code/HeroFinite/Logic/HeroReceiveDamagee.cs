using System.Net.Mail;
using UnityEditor.Analytics;
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


    public void ReceiveDamage(float damage, DamageType damageType, bool shouldTakeHit, bool canDead) // nếu shouldTakeHit = false thì chỉ trừ máu mà không gọi anim hit
    {

        if(shouldTakeHit)
            heroControl.SetIsTakeHit();
        float hpBefore = heroControl.HeroStatRuntime.CurrentHealth;
        float maxHp = heroControl.HeroStatRuntime.MaxHealth;
        float finalDamage = GetDamageAfterArmor(damage);
        if (heroControl != null && heroControl.CanvasTotalDamage != null)
        {
            heroControl.CanvasTotalDamage.TotalDamage += Mathf.RoundToInt(finalDamage);

            // NEW: nếu UI đang show thì mỗi hit update ngay (số chạy)
            if (heroControl.CanvasTotalDamage.IsShowing)
                heroControl.CanvasTotalDamage.UpdateTotalDamage();
        }
        heroControl.HeroStatRuntime.MinusHP((int)finalDamage, damageType);

        
        float hpAfter = heroControl.HeroStatRuntime.CurrentHealth;
        float hpLost = Mathf.Max(0f, hpBefore - hpAfter);
        float hpLost01 = (maxHp > 0f) ? Mathf.Clamp01(hpLost / maxHp) : 0f;

        //mana nhận bằng hp đã mất * 2
        int manaGain = Mathf.RoundToInt(hpLost01 * 2f * heroControl.HeroStatRuntime.MaxMana);
        //nhận mana qua hồn
        foreach (var soul in heroControl.HeroInfo.soulID)
        {
            if (soul ==2)
            {
                FightSoulInfo soulInfo = DatabaseManager.Instance.FightSoulDatabase.GetSoulInfo(soul);
                if (soulInfo != null)
                {
                    HeroInstance heroInstance = PlayerInventory.Instance.GetHeroInstance(heroControl.HeroInfo.ID);
                    int manaAdd = soulInfo.soulValueConfigs[heroInstance.GetLevelSoul(0) - 1].value;
                    manaGain += manaAdd;
                }
            }
           
        }
        if (manaGain > 0)
            heroControl.HeroStatRuntime.GainMana(manaGain);
        if (heroControl.HeroStatRuntime.CurrentHealth <= 0)
        {
            if (canDead)
            {
                isDead = true;
                heroControl.HeroStatRuntime.ClearAllAES();
                heroControl.SetIsDead();
            }
        }
        
        
    }
    public void RefreshTotalDmg()
    {
        heroControl.CanvasTotalDamage.TotalDamage = 0;
    }
    public void SetCanShowTotalDmg()
    {
        heroControl.CanvasTotalDamage.UpdateTotalDamage();
        heroControl.CanvasTotalDamage.Show();
    }
    public void SetCanDead()
    {
        if(heroControl.HeroStatRuntime.CurrentHealth <= 0)
        {
            isDead = true;
            heroControl.HeroStatRuntime.ClearAllAES();
            heroControl.SetIsDead();
        }
    }
    public int GetDamageAfterArmor(float damage)
    {
        float finalDamage = damage;
        float amor = heroControl.HeroStatRuntime.Armor + heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(ModifyStatType.Armor,
                                                        heroControl.HeroStatRuntime.Armor);
        finalDamage = damage * 100 / (100 + amor);
        return Mathf.RoundToInt(finalDamage);
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
