using UnityEngine;

public sealed class HeroStatRuntime : MonoBehaviour
{
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
    public float MaxMana => MaxHealth * 2f;

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
}