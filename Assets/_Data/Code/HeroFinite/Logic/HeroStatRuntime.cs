using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public sealed class HeroStatRuntime : MonoBehaviour
{
    [System.Serializable]
    private struct AESStackState
    {
        public int remainingTurn;
        public int damagePerTurn;

        public AESStackState(int remainingTurn, int damagePerTurn)
        {
            this.remainingTurn = remainingTurn;
            this.damagePerTurn = damagePerTurn;
        }
    }

    [System.Serializable]
    private struct ModifyStatStackState // chỉ lưu khi ability effect type là ModifyStat, để lưu giá trị tăng/giảm tạm thời của buff/debuff
    {
        public int remainingTurn;
        public float modifyValue;

        public ModifyStatStackState(int remainingTurn, float modifyValue)
        {
            this.remainingTurn = remainingTurn;
            this.modifyValue = modifyValue;
        }
    }

    private readonly Dictionary<AbilityEffectType, List<AESStackState>> aesStacksByType =
        new Dictionary<AbilityEffectType, List<AESStackState>>();

    // CHANGED: per ModifyStatType -> per skillName(sourceAbilityName) -> stacks
    private readonly Dictionary<ModifyStatType, Dictionary<string, List<ModifyStatStackState>>> modifyStatStacksByType =
        new Dictionary<ModifyStatType, Dictionary<string, List<ModifyStatStackState>>>();

    public void ClearAllAES()
    {
        if (aesStacksByType.Count == 0)
            return;

        // Copy keys to avoid modifying while iterating
        var keys = new List<AbilityEffectType>(aesStacksByType.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            ClearOldEffect(keys[i]);
        }

        aesStacksByType.Clear();
        SyncAESDebug();
        heroControl.RefreshObservers();
    }

    public bool HasAES(AbilityEffectType type) =>
        aesStacksByType.TryGetValue(type, out var list) && list != null && list.Count > 0;

    public bool HasModifyStat(ModifyStatType type) =>
        modifyStatStacksByType.TryGetValue(type, out var bySkill) && bySkill != null && bySkill.Count > 0;

    public bool HasModifyStat(ModifyStatType type, string sourceAbilityName) =>
        !string.IsNullOrEmpty(sourceAbilityName)
        && modifyStatStacksByType.TryGetValue(type, out var bySkill)
        && bySkill != null
        && bySkill.TryGetValue(sourceAbilityName, out var stacks)
        && stacks != null
        && stacks.Count > 0;

    public List<(AbilityEffectType type, int remainingTurn, int damagePerTurn)> GetAESSnapshot()
    {
        var result = new List<(AbilityEffectType, int, int)>(8);

        foreach (var kv in aesStacksByType)
        {
            var stacks = kv.Value;
            if (stacks == null) continue;

            for (int i = 0; i < stacks.Count; i++)
                result.Add((kv.Key, stacks[i].remainingTurn, stacks[i].damagePerTurn));
        }

        return result;
    }

    // CHANGED: include per-skill info. Optional filter by skill name.
    public List<(ModifyStatType type, string sourceAbilityName, int remainingTurn, float modifyValue)> GetModifyStatSnapshot(string sourceAbilityNameFilter = null)
    {
        var result = new List<(ModifyStatType, string, int, float)>(8);
        bool useFilter = !string.IsNullOrEmpty(sourceAbilityNameFilter);

        foreach (var statKv in modifyStatStacksByType)
        {
            var statType = statKv.Key;
            var bySkill = statKv.Value;
            if (bySkill == null) continue;

            foreach (var skillKv in bySkill)
            {
                string skillName = skillKv.Key;
                if (useFilter && skillName != sourceAbilityNameFilter)
                    continue;

                var stacks = skillKv.Value;
                if (stacks == null) continue;

                for (int i = 0; i < stacks.Count; i++)
                    result.Add((statType, skillName, stacks[i].remainingTurn, stacks[i].modifyValue));
            }
        }

        return result;
    }

    public void MinusRemainTurn(AbilityEffectType types)
    {
        if (aesStacksByType.Count == 0) return;
        if (!aesStacksByType.ContainsKey(types))
            return;

        var keys = new List<AbilityEffectType> { types };

        for (int k = 0; k < keys.Count; k++)
        {
            var type = keys[k];

            if (!aesStacksByType.TryGetValue(type, out var stacks) || stacks == null || stacks.Count == 0)
                continue;

            // giảm remaining của từng stack (mỗi stack có remaining riêng)
            for (int i = stacks.Count - 1; i >= 0; i--)
            {
                var s = stacks[i];
                s.remainingTurn = Mathf.Max(0, s.remainingTurn - 1);

                if (s.remainingTurn <= 0)
                    stacks.RemoveAt(i);
                else
                    stacks[i] = s;
            }

            // nếu hết toàn bộ stack của type này => remove type + cancel UI effect
            if (stacks.Count == 0)
            {
                aesStacksByType.Remove(type);

                switch (type)
                {
                    case AbilityEffectType.Burn:
                        CancelBurn();
                        break;
                    case AbilityEffectType.Rooted:
                        heroControl.RefreshObservers("canDisappear", true);
                        heroControl.HeroEventt.CallCancelStopAnim();
                        heroControl.CanAttackInBattle = true;
                        break;
                }
            }
        }

        SyncAESDebug();
    }

    // CHANGED: tick per stat per skill
    public void MinusModifyStatRemainTurn(ModifyStatType type)
    {
        if (!modifyStatStacksByType.TryGetValue(type, out var bySkill) || bySkill == null || bySkill.Count == 0)
            return;

        var skillKeys = new List<string>(bySkill.Keys);
        for (int k = 0; k < skillKeys.Count; k++)
        {
            string skillName = skillKeys[k];
            if (!bySkill.TryGetValue(skillName, out var stacks) || stacks == null || stacks.Count == 0)
                continue;

            for (int i = stacks.Count - 1; i >= 0; i--)
            {
                var s = stacks[i];
                s.remainingTurn = Mathf.Max(0, s.remainingTurn - 1);

                if (s.remainingTurn <= 0)
                    stacks.RemoveAt(i);
                else
                    stacks[i] = s;
            }

            if (stacks.Count == 0)
            {
                bySkill.Remove(skillName);
            }
               
        }

        if (bySkill.Count == 0)
            modifyStatStacksByType.Remove(type);

        SyncModifyStatDebug();
    }

    public void MinusAllModifyStatRemainTurn()
    {
        if (modifyStatStacksByType.Count == 0) return;

        var keys = new List<ModifyStatType>(modifyStatStacksByType.Keys);
        for (int k = 0; k < keys.Count; k++)
            MinusModifyStatRemainTurn(keys[k]);

        SyncModifyStatDebug();
    }

    public void ApplyAES(AbilityEffectType type, int remainingTurn, int damagePerTurn, int maxStacks)
    {
        if (remainingTurn <= 0) return;
        if (maxStacks <= 0) return;

        if (type == AbilityEffectType.ModifyStat) return;

        if (!aesStacksByType.TryGetValue(type, out var stacks) || stacks == null)
        {
            stacks = new List<AESStackState>(maxStacks);
            aesStacksByType[type] = stacks;

            ApplyUIEffect(type);
            heroControl.RefreshObservers(type);
        }

        // nếu đã max stack: bỏ stack cũ nhất (thường rule: drop oldest)
        while (stacks.Count >= maxStacks)
            stacks.RemoveAt(0);

        int finalDamagePerTurn = 0;
        if (damagePerTurn != 0)
        {
            finalDamagePerTurn = heroControl.HeroReceiveDamagee.GetDamageAfterArmor(damagePerTurn);
        }

        if (type == AbilityEffectType.Rooted)
        {
            heroControl.RefreshObservers("canDisappear", false);
            heroControl.CanAttackInBattle = false;
        }

        stacks.Add(new AESStackState(remainingTurn, finalDamagePerTurn));

        SyncAESDebug();
    }

    // CHANGED: maxStacks is PER SKILL (sourceAbilityName) not global per stat.
    // If same skill reaches cap => refresh duration only; do NOT apply stat again.
    public void ApplyModifyStat(string sourceAbilityName, ModifyStatType type, int remainingTurn, float modifyValue, int maxStacks, bool instant = true)
    {
        if (string.IsNullOrEmpty(sourceAbilityName)) return;
        if (remainingTurn <= 0) return;
        if (maxStacks <= 0) return;

        if (!modifyStatStacksByType.TryGetValue(type, out var bySkill) || bySkill == null)
        {
            bySkill = new Dictionary<string, List<ModifyStatStackState>>();
            modifyStatStacksByType[type] = bySkill;
        }

        if (!bySkill.TryGetValue(sourceAbilityName, out var stacks) || stacks == null)
        {
            stacks = new List<ModifyStatStackState>(maxStacks);
            bySkill[sourceAbilityName] = stacks;
        }

        // per-skill cap reached => refresh duration (max) and exit (no re-apply)
        if (stacks.Count >= maxStacks)
        {
            var s = stacks[0];
            s.remainingTurn = Mathf.Max(s.remainingTurn, remainingTurn);
            stacks[0] = s;

            SyncModifyStatDebug();
            return;
        }

        stacks.Add(new ModifyStatStackState(remainingTurn, modifyValue));

        // Apply to runtime stat immediately (your current stat system is "apply now", not derived each frame)
        //ApplyStats(type, modifyValue, instant);

        SyncModifyStatDebug();
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

    public void GainValueBySoul(HeroInstance instance, FightSoulInfo soulInfo)
    {
        if (soulInfo.fightSoulType == FightSoulType.ManaSoul)
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
                GainHP((int)value, DamageType.normalDamage, instant);
                break;
            case ModifyStatType.Armor:
                GainArmor(value);
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

    public void GainHPMax(float value, bool instant = false)
    {
        finalStat.health *= (1 + value);
        currentHealth = finalStat.health;
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;
        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
    }

    public void GainArmor(float value)
    {
        finalStat.armor *= (1 + value / 100);
    }

    public void GainCritRate(float value, bool instant = false)
    {
        finalStat.critRate += value;
        if (instant) return;
    }

    public void GainDamage(float value)
    {
        finalStat.damage *= (1 + value);
    }

    public void GainMana(int value, bool instant = false)
    {
        currentMana += value;
        if (currentMana >= 1000)
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

    public void GainHP(int value, DamageType damageType, bool instant = false)
    {
        // NEW: apply HealingRate (%)
        // HealingRate is stored in modifyStatStacksByType as percent (20 = +20%).
        float totalHealingRatePercent = GetTotalModifyPercent(ModifyStatType.HealingRate);
        float scaledValueF = value * (1f + (totalHealingRatePercent / 100f));
        int scaledValue = Mathf.RoundToInt(scaledValueF);

        CurrentHealth += scaledValue;
        if (currentHealth >= MaxHealth)
        {
            currentHealth = MaxHealth;
        }
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;

        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
        heroControl.RefreshObservers(HPNotifyType.HPPlus, damageType, scaledValue);
    }


    public void MinusHP(int value, DamageType damageType, bool instant = false)
    {
        CurrentHealth -= value;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;

        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
        heroControl.RefreshObservers(HPNotifyType.HPMinus, damageType, (int)value);
    }

    public void ApplyUIEffect(AbilityEffectType effectType)
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
        ClearOldEffect(AbilityEffectType.Burn);
        EffectManager.Instance.Spawn(
            AbilityEffectType.Burn,
            heroControl.SpriteEffect.transform
        );
        heroControl.SpriteEffect.gameObject.SetActive(true);
    }

    public void ApplyEartEffect()
    {
        if (heroControl.SpriteRenderer.enabled)
        {
            heroControl.SpriteRenderer.enabled = false;
        }
        else
        {
            heroControl.SpriteRenderer.enabled = true;
        }
    }

    void CancelBurn()
    {
        heroControl.SpriteEffect.gameObject.SetActive(false);
    }

    void ClearOldEffect(AbilityEffectType type)
    {
        foreach (Transform child in heroControl.SpriteEffect.transform)
        {
            Effect_Item effect_item = child.GetComponent<Effect_Item>();
            if (effect_item != null && effect_item.GetAbilityType() == type)
                Destroy(child.gameObject);
        }
    }



    
    public float GetTotalModifyPercent(ModifyStatType type)
    {
        float totalPercent = 0f;

        if (!modifyStatStacksByType.TryGetValue(type, out var bySkill) || bySkill == null || bySkill.Count == 0)
            return 0f;

        foreach (var skillKv in bySkill)
        {
            var stacks = skillKv.Value;
            if (stacks == null) continue;

            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].remainingTurn <= 0) continue;
                totalPercent += stacks[i].modifyValue;
            }
        }

        return totalPercent;
    }

    // UPDATED: giữ hàm cũ, nhưng cho lấy luôn totalPercent nếu cần debug
    public float GetFinalValueAfterModifyStat(ModifyStatType type, float baseValue)
    {
        float totalPercent = GetTotalModifyPercent(type);
        return baseValue * (1f + totalPercent / 100f);
    }




    // ===================== Debug (Inspector) =====================
    [System.Serializable]
    private struct AESDebugItem
    {
        public AbilityEffectType type;

        [Header("Stacks")]
        public List<AESStackState> stacks;

        public int TotalDamagePerTurn()
        {
            if (stacks == null) return 0;
            int sum = 0;
            for (int i = 0; i < stacks.Count; i++)
                sum += stacks[i].damagePerTurn;
            return sum;
        }
    }

    [System.Serializable]
    private struct ModifyStatDebugItem
    {
        public ModifyStatType type;
        public string sourceAbilityName;
        public List<ModifyStatStackState> stacks;
    }

    [Header("Debug (Inspector)")]
    [SerializeField] private List<AESDebugItem> aesDebug = new List<AESDebugItem>();

    [Header("Debug ModifyStat (Inspector)")]
    [SerializeField] private List<ModifyStatDebugItem> modifyStatDebug = new List<ModifyStatDebugItem>();

    private void SyncAESDebug()
    {
        aesDebug.Clear();

        foreach (var kv in aesStacksByType)
        {
            var stacks = kv.Value;
            if (stacks == null) continue;

            // NOTE: copy list để inspector hiển thị ổn định và tránh reference tới list runtime
            var copy = new List<AESStackState>(stacks.Count);
            for (int i = 0; i < stacks.Count; i++)
                copy.Add(stacks[i]);

            aesDebug.Add(new AESDebugItem
            {
                type = kv.Key,
                stacks = copy
            });
        }
    }

    private void SyncModifyStatDebug()
    {
        modifyStatDebug.Clear();

        foreach (var statKv in modifyStatStacksByType)
        {
            var type = statKv.Key;
            var bySkill = statKv.Value;
            if (bySkill == null) continue;

            foreach (var skillKv in bySkill)
            {
                var skillName = skillKv.Key;
                var stacks = skillKv.Value;
                if (stacks == null) continue;

                var copy = new List<ModifyStatStackState>(stacks.Count);
                for (int i = 0; i < stacks.Count; i++)
                    copy.Add(stacks[i]);

                modifyStatDebug.Add(new ModifyStatDebugItem
                {
                    type = type,
                    sourceAbilityName = skillName,
                    stacks = copy
                });
            }
        }
    }
}