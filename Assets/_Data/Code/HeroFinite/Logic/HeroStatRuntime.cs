using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]

public sealed class HeroStatRuntime : MonoBehaviour
{
    private struct AbilityEffectState
    {
        public int remainingTurn;
        public int damagePerTurn;

        public AbilityEffectState(int remainingTurn, int damagePerTurn)
        {
            this.remainingTurn = remainingTurn;
            this.damagePerTurn = damagePerTurn;
        }
    }
    private readonly Dictionary<AbilityEffectType, AbilityEffectState> dotByType =
        new Dictionary<AbilityEffectType, AbilityEffectState>();

    public bool HasAES(AbilityEffectType type) => dotByType.ContainsKey(type);

    public void ApplyAES(AbilityEffectType type, int remainingTurn, int damagePerTurn)
    {
        Debug.Log("vo apply aes");
        if (remainingTurn <= 0 || damagePerTurn < 0) return;
        Debug.Log("qua dc check <=0");
        // Only allow DOT-like types here (avoid ModifyStat, Stun...)
        if (type != AbilityEffectType.Burn)
            return;
        ApplyUIEffect(type);
        if (dotByType.TryGetValue(type, out var cur))
        {
            // refresh rule: lấy duration lớn hơn + damage lớn hơn
            cur.remainingTurn = Mathf.Max(cur.remainingTurn, remainingTurn);
            cur.damagePerTurn = Mathf.Max(cur.damagePerTurn, damagePerTurn);
            dotByType[type] = cur;
        }
        else
        {
            dotByType[type] = new AbilityEffectState(remainingTurn, damagePerTurn);
        }
    }
    [Header("Refs")]
    [SerializeField] private HeroControl heroControl;

    [Header("Source Data")]
    [SerializeField] private HeroInfo baseInfo;
    public HeroInfo BaseInfo => baseInfo;

    [SerializeField] private HeroInstance heroInstance;
    public HeroInstance HeroInstance => heroInstance;

    [Header("Calculated Stat (final)")]
    [SerializeField] private HeroStat finalStat;
    public HeroStat FinalStat => finalStat;

    [Header("Runtime Values")]
    [SerializeField] private float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Max(0f, value); }

    [SerializeField] private float currentMana;
    public float CurrentMana { get => currentMana; set => currentMana = Mathf.Max(0f, value); }

    public float MaxHealth => finalStat != null ? finalStat.health : 0f;
    public float MaxMana => 1000f;

    public float Damage => finalStat != null ? finalStat.damage : 0f;
    public float Armor => finalStat != null ? finalStat.armor : 0f;

    public float CritRate => finalStat != null ? finalStat.critRate : 0f;
    public float CritDamage => finalStat != null ? finalStat.critDamage : 0f;

    public float Speed => finalStat != null ? finalStat.speed : 0f;

    private void Awake()
    {
        if (heroControl == null)
            heroControl = GetComponent<HeroControl>();
        baseInfo = heroControl != null ? heroControl.HeroInfo : null;
    }

    public void Init(HeroInfo info, HeroInstance instance, HeroGrowthConfig growth)
    {
        baseInfo = info;
        heroInstance = instance;

        if (baseInfo == null)
        {
            finalStat = null;
            currentHealth = 0f;
            currentMana = 0f;
            return;
        }

        if (heroInstance != null && growth != null)
        {
            finalStat = HeroStatCalculator.Calculate(baseInfo, heroInstance, growth);
        }
        else
        {
            finalStat = new HeroStat
            {
                damage = baseInfo.damage,
                health = baseInfo.health,
                armor = baseInfo.armor,
                critRate = baseInfo.criticalRate,
                critDamage = baseInfo.criticalDamageRate,
                speed = baseInfo.speed,
                power = 0f
            };
        }

        currentHealth = finalStat.health;
        currentMana = 0f;
    }
    public void GainValueBySoul(HeroInstance instance ,FightSoulInfo soulInfo)
    {
        if(soulInfo.fightSoulType == FightSoulType.ManaSoul)
        {

            if (soulInfo.soulValueConfigs == null || soulInfo.soulValueConfigs.Length == 0)
                return;
            GainMana(soulInfo.soulValueConfigs[instance.GetLevelSoul(0) - 1].value, true);
        }
    }
    public void ApplyStats(ModifyStatType type, float value, bool instant)
    {
        switch (type)
        {
            case ModifyStatType.Damage:
                GainDamage(value);
                break;
            case ModifyStatType.Health:
                GainHPMax(value, instant);
                break;
            case ModifyStatType.Armor:
                
                break;
            case ModifyStatType.CritRate:
                GainCritRate(value, instant);

                break;
            case ModifyStatType.CritDamage:
                
                break;
            case ModifyStatType.Speed:
                
                break;
        }
    }
    public void GainHPMax(float  value, bool instant = false)
    {
        finalStat.health *= (1 + value);
        currentHealth = finalStat.health;
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;
        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);

    }
    public void GainCritRate(float value, bool instant = false)
    {
        finalStat.critRate +=value;
        if (instant) return;
       
    }
    public void GainDamage(float  value)
    {
        finalStat.damage *= (1 + value);
        

    }
    public void GainMana(int value, bool instant = false)
    {
        currentMana += value;
        if(currentMana >= 1000)
        {
            currentMana = 1000;
        }
        if (instant) return;
        float mana01 = currentMana / (float)MaxMana;
      
        heroControl.RefreshObservers(HeroNotifyType.ManaChanged, mana01);
      
    }
    public void MinusMana(int value)
    {
        currentMana -= value;
        if (currentMana <= 0)
        {
            currentMana = 0;
        }
        float mana01 = currentMana / (float)MaxMana;

        heroControl.RefreshObservers(HeroNotifyType.ManaChanged, mana01);
    }

    public void GainHP(int value, bool instant = false)
    {
        CurrentHealth += value;
        if (currentHealth >= MaxHealth)
        {
            currentHealth = MaxHealth;
        }
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;

        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
    }
    public void MinusHP(int value , DamageType damageType, bool instant = false)
    {
        CurrentHealth -= value;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;

        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
        heroControl.RefreshObservers(HPNotifyType.HPMinus,  damageType, (int)value);
        
    }

    public void ApplyUIEffect(AbilityEffectType effectType )
    {
        switch (effectType)
        {
            case AbilityEffectType.Burn:
                ApplyBurn();
                break;
        }
    }
    void ApplyBurn()
    {
        heroControl.SpriteEffect.color = new Color(
            255f / 255f,
            50f / 255f,
            0f / 255f,
            150f / 255f
        );

        heroControl.SpriteEffect.gameObject.SetActive(true);
    }

}