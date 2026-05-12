using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEditor.Analytics;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MPE;
using UnityEngine;

public class HeroReceiveDamagee : MonoBehaviour, IObserver
{
  
    protected List<string> nameObjectEffect = new List<string>();
    

    [SerializeField] protected HeroControl heroControl;
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


    public virtual float ReceiveDamage(float damage, DamageType damageType,
        bool shouldTakeHit, bool canDead,
        HeroControl attacker = null) // nếu shouldTakeHit = false thì chỉ trừ máu mà không gọi anim hit
    {

        CallTakeHit(shouldTakeHit);
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
            if (soul == 2)
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
            heroControl.HeroStatRuntime.GainMana(manaGain, true);
        if (damageType == DamageType.turnDamage)// khi bị cháy
        {
            if (heroControl.HeroStatRuntime.CurrentHealth <= 0 && !heroControl.CanDodge)
            {

                if (canDead)
                {

                    HandleDead(attacker);
                    heroControl.HeroStatRuntime.ClearAllAES();
                    //heroControl.IsFinished = false;
                    heroControl.SetIsDead();
                }
            }
        }
        if (heroControl.HeroInfo.ID == 55)
        {
            attacker.HeroReceiveDamagee.ReceiveDamage(finalDamage * 0.25f, DamageType.counterDamage, false, false);
        }
        
        if(damageType != DamageType.counterDamage || damageType != DamageType.turnDamage)
            CallWaitForAttackerFinished(attacker);
        return finalDamage;
        
        
    }
    public virtual void CallWaitForAttackerFinished(HeroControl attacker)
    {
        StartCoroutine(WaitForAttackerFinished(attacker));
    }
    IEnumerator WaitForAttackerFinished(HeroControl attacker)
    {
        yield return new WaitUntil(() => attacker.IsFinished);
        if (isDead) yield break;
        heroControl.IsFinished = true;
        if (heroControl.HeroInfo.ID == 513)
        {
            List<AbilityEffect> effectsSpecial = heroControl.HeroInfo.passive.GetEffectsOnSpecial();
            float finalDamage = heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(ModifyStatType.Damage, heroControl.HeroInfo.damage);
            if (effectsSpecial == null || effectsSpecial.Count == 0) yield break ;
            float damagePerTurn = effectsSpecial[0].modifyValue/100f * finalDamage ;
            attacker.HeroStatRuntime.ApplyAES(
                    heroControl.HeroInfo.passive.abilityName,
                    AbilityEffectType.Burn,
                    effectsSpecial[0].durationTurn,
                    (int)damagePerTurn,
                    effectsSpecial[0].stackCount,
                    heroControl
                );
        }
        heroControl.HeroEventt.RefreshHasShown();
        heroControl.HeroEventt.FirstAttack = false;
    }
    public virtual void CallTakeHit(bool shouldTakeHit)
    {
        if (shouldTakeHit)
            heroControl.SetIsTakeHit();
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
    public void SetCanDead(HeroControl attacker)
    {
        if(heroControl.HeroStatRuntime.CurrentHealth <= 0 && !heroControl.CanDodge)
        {
            HandleDead(attacker);
            
            heroControl.HeroStatRuntime.ClearAllAES();
            //heroControl.IsFinished = false;
            heroControl.SetIsDead();
        }
    }
    
    public int GetDamageAfterArmor(float damage)
    {
        float armor = heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
            ModifyStatType.Armor,
            heroControl.HeroStatRuntime.Armor
        );

        float finalDamage = damage * 100f / (100f + armor);
        return Mathf.RoundToInt(finalDamage);
    }

    public void CallStopAnim(float duration)
    {
        
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
    //==Effect riêng cho từng hero
    protected virtual void ApplyEffectForHero()
    {
        CallSpawnEffectHero(null, new Vector3(0,0,0));
    }

    protected virtual void HandleDead(HeroControl attacker = null)
    {
        isDead = true;
    }
    public virtual void CallSpawnEffectHero(string nameObject, Vector3 positionSpawn)
    {
        if (nameObjectEffect.Contains(nameObject))
            return;
        GameObject spawnEffect = EffectManager.Instance.Spawn(
              nameObject,
              HeroControl.ListEffect.transform
          );
        spawnEffect.name = nameObject;
        if (spawnEffect != null)
        {

            Transform effectTransform = spawnEffect.transform;
            Vector3 heroScale = HeroControl.transform.localScale;
            Vector3 effectScale = effectTransform.localScale;

            effectTransform.localScale = new Vector3(
                effectScale.x / heroScale.x,
                effectScale.y / heroScale.y,
                1
            );


            Vector3 effectPos = effectTransform.position;
            effectTransform.localPosition = positionSpawn;
        }
        nameObjectEffect.Add(nameObject);
    }

    public void ClearEffectByName(string nameObject)
    {
        nameObjectEffect.Remove(nameObject);
        foreach (Transform child in HeroControl.ListEffect.transform)
        {
            if (child.name == nameObject)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }


}
