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
        public HeroControl attacker;

        public AESStackState(int remainingTurn, int damagePerTurn, HeroControl attacker)
        {
            this.remainingTurn = remainingTurn;
            this.damagePerTurn = damagePerTurn;
            this.attacker = attacker;
        }
    }

    [System.Serializable]
    private struct ModifyStatStackState // chỉ lưu khi ability effect type là ModifyStat, để lưu giá trị tăng/giảm tạm thời của buff/debuff
    {
        public int remainingTurn;
        public float modifyValue;
        public HeroControl attacker;

        public ModifyStatStackState(int remainingTurn, float modifyValue, HeroControl attacker)
        {
            this.remainingTurn = remainingTurn;
            this.modifyValue = modifyValue;
            this.attacker = attacker;
        }
    }

    // CHANGED: per AbilityEffectType -> per skillName(sourceAbilityName) -> stacks
    private readonly Dictionary<AbilityEffectType, Dictionary<string, List<AESStackState>>> aesStacksByType =
        new Dictionary<AbilityEffectType, Dictionary<string, List<AESStackState>>>();

    // CHANGED: per ModifyStatType -> per skillName(sourceAbilityName) -> stacks
    private readonly Dictionary<ModifyStatType, Dictionary<string, List<ModifyStatStackState>>> modifyStatStacksByType =
        new Dictionary<ModifyStatType, Dictionary<string, List<ModifyStatStackState>>>();

    Dictionary<ModifyStatType, int> dicOnStartBattle = new Dictionary<ModifyStatType, int>();
    public Dictionary<ModifyStatType, int> GetDicOnStartBattle()
    {
        return dicOnStartBattle;
    }

    public bool HasAnyAES()
    {
        if (aesStacksByType == null || aesStacksByType.Count == 0)
            return false;

        foreach (var typeKv in aesStacksByType)
        {
            var bySkill = typeKv.Value;
            if (bySkill == null || bySkill.Count == 0)
                continue;

            foreach (var skillKv in bySkill)
            {
                var stacks = skillKv.Value;
                if (stacks != null && stacks.Count > 0)
                    return true;
            }
        }

        return false;
    }

    public void ClearAllAES()
    {
        if (aesStacksByType.Count == 0)
            return;

        // Clear old VFX/UI for all AES types
        var typeKeys = new List<AbilityEffectType>(aesStacksByType.Keys);
        for (int i = 0; i < typeKeys.Count; i++)
        {
            ClearOldEffect(typeKeys[i]);

            // Cancel UI effect based on type (only if existed)
            switch (typeKeys[i])
            {
                case AbilityEffectType.Burn:
                    CancelBurn();
                    break;
                case AbilityEffectType.Bleeding:
                    CancelBleeding();
                    break;
                case AbilityEffectType.Poison:
                    CancelPoison();
                    break;
                case AbilityEffectType.Rooted:
                    heroControl.RefreshObservers("canDisappear", true);
                    heroControl.HeroEventt.CallCancelStopAnim();
                    heroControl.CanAttackInBattle = true;
                    break;
                case AbilityEffectType.Stun:
                    CancelStun();
                    heroControl.HeroEventt.CallCancelStopAnim();
                    heroControl.CanAttackInBattle = true;
                    break;
                case AbilityEffectType.Paralysis:
                    CancelParalysis();
                    heroControl.HeroEventt.CallCancelStopAnim();
                    heroControl.CanAttackInBattle = true;
                    break;
            }
        }

        aesStacksByType.Clear();
        SyncAESDebug();
        heroControl.RefreshObservers();
    }

    public bool HasAES(AbilityEffectType type) =>
        aesStacksByType.TryGetValue(type, out var bySkill) && bySkill != null && bySkill.Count > 0;

    public bool HasAES(AbilityEffectType type, string sourceAbilityName) =>
        !string.IsNullOrEmpty(sourceAbilityName)
        && aesStacksByType.TryGetValue(type, out var bySkill)
        && bySkill != null
        && bySkill.TryGetValue(sourceAbilityName, out var stacks)
        && stacks != null
        && stacks.Count > 0;

    public bool HasModifyStat(ModifyStatType type) =>
        modifyStatStacksByType.TryGetValue(type, out var bySkill) && bySkill != null && bySkill.Count > 0;

    public bool HasModifyStat(ModifyStatType type, string sourceAbilityName) =>
        !string.IsNullOrEmpty(sourceAbilityName)
        && modifyStatStacksByType.TryGetValue(type, out var bySkill)
        && bySkill != null
        && bySkill.TryGetValue(sourceAbilityName, out var stacks)
        && stacks != null
        && stacks.Count > 0;

    // CHANGED: include per-skill info. Optional filter by skill name.
    public List<(AbilityEffectType type, string sourceAbilityName, int remainingTurn, int damagePerTurn, HeroControl attacker)> GetAESSnapshot(string sourceAbilityNameFilter = null)
    {
        var result = new List<(AbilityEffectType, string, int, int, HeroControl)>(8);
        bool useFilter = !string.IsNullOrEmpty(sourceAbilityNameFilter);

        foreach (var typeKv in aesStacksByType)
        {
            var type = typeKv.Key;
            var bySkill = typeKv.Value;
            if (bySkill == null) continue;

            foreach (var skillKv in bySkill)
            {
                string skillName = skillKv.Key;
                if (useFilter && skillName != sourceAbilityNameFilter)
                    continue;

                var stacks = skillKv.Value;
                if (stacks == null) continue;

                for (int i = 0; i < stacks.Count; i++)
                    result.Add((type, skillName, stacks[i].remainingTurn, stacks[i].damagePerTurn, stacks[i].attacker));
            }
        }

        return result;
    }

    // CHANGED: include per-skill info. Optional filter by skill name.
    public List<(ModifyStatType type, string sourceAbilityName, int remainingTurn, float modifyValue, HeroControl attacker)> GetModifyStatSnapshot(string sourceAbilityNameFilter = null)
    {
        var result = new List<(ModifyStatType, string, int, float, HeroControl)>(8);
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
                    result.Add((statType, skillName, stacks[i].remainingTurn, stacks[i].modifyValue, stacks[i].attacker));
            }
        }

        return result;
    }

    // CHANGED: tick per type per skill
    public void MinusRemainTurn(AbilityEffectType type)
    {
        if (!aesStacksByType.TryGetValue(type, out var bySkill) || bySkill == null || bySkill.Count == 0)
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
                bySkill.Remove(skillName);
        }

        // if no skill remains for this AES type => remove type and cancel UI effect
        if (bySkill.Count == 0)
        {
            aesStacksByType.Remove(type);

            switch (type)
            {
                case AbilityEffectType.Burn:
                    CancelBurn();
                    break;
                case AbilityEffectType.Poison:
                    CancelPoison();
                    break;
                case AbilityEffectType.Bleeding:
                    CancelBleeding();
                    break;
                case AbilityEffectType.Rooted:
                    heroControl.RefreshObservers("canDisappear", true);
                    heroControl.HeroEventt.CallCancelStopAnim();
                    heroControl.CanAttackInBattle = true;
                    break;
                case AbilityEffectType.Stun:
                    CancelStun();
                    heroControl.HeroEventt.CallCancelStopAnim();
                    heroControl.CanAttackInBattle = true;
                    break;
                case AbilityEffectType.Paralysis:
                    CancelParalysis();
                    heroControl.HeroEventt.CallCancelStopAnim();
                    heroControl.CanAttackInBattle = true;
                    break;
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

    // NEW: remove AES by skill (mirrors RemoveModifyStatBySkill pattern)
    public void RemoveAESBySkill(string sourceAbilityName, AbilityEffectType type)
    {
        if (string.IsNullOrEmpty(sourceAbilityName)) return;
        if (!aesStacksByType.TryGetValue(type, out var bySkill) || bySkill == null)
            return;

        if (bySkill.Remove(sourceAbilityName))
        {
            if (bySkill.Count == 0)
            {
                aesStacksByType.Remove(type);

                switch (type)
                {
                    case AbilityEffectType.Burn:
                        CancelBurn();
                        break;
                    case AbilityEffectType.Poison:
                        CancelPoison();
                        break;
                    case AbilityEffectType.Bleeding:
                        CancelBleeding();
                        break;
                    case AbilityEffectType.Rooted:
                        heroControl.RefreshObservers("canDisappear", true);
                        heroControl.HeroEventt.CallCancelStopAnim();
                        heroControl.CanAttackInBattle = true;
                        break;
                    case AbilityEffectType.Stun:
                        CancelStun();
                        heroControl.HeroEventt.CallCancelStopAnim();
                        heroControl.CanAttackInBattle = true;
                        break;
                }
            }

            SyncAESDebug();
        }
    }

    public void RemoveModifyStatBySkill(string sourceAbilityName, ModifyStatType type)
    {
        if (string.IsNullOrEmpty(sourceAbilityName)) return;

        if (!modifyStatStacksByType.TryGetValue(type, out var bySkill) || bySkill == null)
            return;

        if (bySkill.TryGetValue(sourceAbilityName, out var stacks) && stacks != null)
        {
            // Xóa toàn bộ stacks của skill này cho type này
            bySkill.Remove(sourceAbilityName);
            Debug.Log($"[ModifyStat] Removed {sourceAbilityName} for type {type}");

            // Nếu type này không còn skill nào, xóa luôn type
            if (bySkill.Count == 0)
            {
                modifyStatStacksByType.Remove(type);
                Debug.Log($"[ModifyStat] Type {type} is now empty, removing it");
            }

            SyncModifyStatDebug();
        }
    }

    // CHANGED: per-skill cap (like ModifyStat). If cap reached => refresh duration only.
    // NEW: thêm idAttacker để lưu người gây effect
    //public void ApplyAES(string sourceAbilityName, AbilityEffectType type, int remainingTurn, int damagePerTurn, int maxStacks, HeroControl attacker)
    //{
    //    bool hasShown = false;
    //    if (string.IsNullOrEmpty(sourceAbilityName)) return;
    //    if (remainingTurn <= 0) return;
    //    if (maxStacks <= 0) return;

    //    if (type == AbilityEffectType.ModifyStat) return;
    //    float controlFreeValue = heroControl.HeroInfo.controlFree / 100;
    //    if (Random.value < controlFreeValue) return;

    //    if (!aesStacksByType.TryGetValue(type, out var bySkill) || bySkill == null)
    //    {
    //        bySkill = new Dictionary<string, List<AESStackState>>();
    //        aesStacksByType[type] = bySkill;

    //        ApplyUIEffect(type);
    //        if (!hasShown)
    //        {
    //            heroControl.RefreshObservers(type);
    //            hasShown = true;
    //        }

    //    }

    //    if (!bySkill.TryGetValue(sourceAbilityName, out var stacks) || stacks == null)
    //    {
    //        stacks = new List<AESStackState>(maxStacks);
    //        bySkill[sourceAbilityName] = stacks;
    //        if (!hasShown)
    //        {
    //            heroControl.RefreshObservers(type);
    //            hasShown = true;
    //        }

    //    }


    //    int finalDamagePerTurn = 0;
    //    if (damagePerTurn != 0)
    //    {
    //        finalDamagePerTurn = heroControl.HeroReceiveDamagee.GetDamageAfterArmor(damagePerTurn);
    //    }

    //    // Apply immediate restrictions (only need once globally, but safe to set each time)
    //    if (type == AbilityEffectType.Rooted)
    //    {
    //        heroControl.RefreshObservers("canDisappear", false);
    //        heroControl.CanAttackInBattle = false;
    //    }
    //    else if (type == AbilityEffectType.Stun)
    //    {
    //        heroControl.CanAttackInBattle = false;
    //    }
    //    else if (type == AbilityEffectType.Paralysis)
    //    {
    //        heroControl.CanAttackInBattle = false;
    //    }

    //    // per-skill cap reached => refresh duration (max) and exit (no new stack)
    //    if (stacks.Count >= maxStacks)
    //    {
    //        var s = stacks[0];
    //        s.remainingTurn = Mathf.Max(s.remainingTurn, remainingTurn);
    //        stacks[0] = s;
    //        if (!hasShown)
    //        {
    //            heroControl.RefreshObservers(type);
    //            hasShown = true;
    //        }
    //        SyncAESDebug();
    //        return;
    //    }

    //    stacks.Add(new AESStackState(remainingTurn, finalDamagePerTurn, attacker));
    //    if (!hasShown)
    //    {
    //        heroControl.RefreshObservers(type);
    //        hasShown = true;
    //    }

    //    SyncAESDebug();
    //}

    public void ApplyAES(string sourceAbilityName, AbilityEffectType type, int remainingTurn, int damagePerTurn, int maxStacks, HeroControl attacker)
    {
        if(heroControl.HeroEventt.HasShown) return;
        
        if (string.IsNullOrEmpty(sourceAbilityName)) return;
        if (remainingTurn <= 0) return;
        if (maxStacks <= 0) return;

        if (type == AbilityEffectType.ModifyStat) return;
        float controlFreeValue = heroControl.HeroInfo.controlFree / 100;
        if (Random.value < controlFreeValue) return;

        if (!aesStacksByType.TryGetValue(type, out var bySkill) || bySkill == null)
        {
            bySkill = new Dictionary<string, List<AESStackState>>();
            aesStacksByType[type] = bySkill;

            ApplyUIEffect(type);
            if (!heroControl.HeroEventt.HasShown)
            {
                heroControl.RefreshObservers(type);
                heroControl.HeroEventt.HasShown = true;
            }

        }

        if (!bySkill.TryGetValue(sourceAbilityName, out var stacks) || stacks == null)
        {
            stacks = new List<AESStackState>(maxStacks);
            bySkill[sourceAbilityName] = stacks;
            if (!heroControl.HeroEventt.HasShown)
            {
                heroControl.RefreshObservers(type);
                heroControl.HeroEventt.HasShown = true;
            }

        }



        // Apply immediate restrictions (only need once globally, but safe to set each time)
        if (type == AbilityEffectType.Rooted)
        {
            heroControl.RefreshObservers("canDisappear", false);
            heroControl.CanAttackInBattle = false;
        }
        else if (type == AbilityEffectType.Stun)
        {
            heroControl.CanAttackInBattle = false;
        }
        else if (type == AbilityEffectType.Paralysis)
        {
            heroControl.CanAttackInBattle = false;
        }

        // per-skill cap reached => refresh duration (max) and exit (no new stack)
        if (stacks.Count >= maxStacks)
        {
            var s = stacks[0];
            s.remainingTurn = Mathf.Max(s.remainingTurn, remainingTurn);
            stacks[0] = s;
            if (!heroControl.HeroEventt.HasShown)
            {
                heroControl.RefreshObservers(type);
                heroControl.HeroEventt.HasShown = true;
            }
            SyncAESDebug();
            return;
        }

        stacks.Add(new AESStackState(remainingTurn, damagePerTurn, attacker));
        if (!heroControl.HeroEventt.HasShown)
        {
            heroControl.RefreshObservers(type);
            heroControl.HeroEventt.HasShown = true;
        }

        SyncAESDebug();
    }
    public void ApplyModifyStat(string sourceAbilityName, ModifyStatType type, int remainingTurn, float modifyValue, int maxStacks, HeroControl attacker, bool instant = true)
    {
        bool hasShown = false;
        if (string.IsNullOrEmpty(sourceAbilityName)) return;
        if (remainingTurn <= 0) return;
        if (maxStacks <= 0) return;

        if (!modifyStatStacksByType.TryGetValue(type, out var bySkill) || bySkill == null)
        {
            bySkill = new Dictionary<string, List<ModifyStatStackState>>();
            modifyStatStacksByType[type] = bySkill;
            if (!hasShown)
            {
                heroControl.RefreshObservers(type, (int)modifyValue);
                hasShown = true;
            }
        }

        if (!bySkill.TryGetValue(sourceAbilityName, out var stacks) || stacks == null)
        {
            stacks = new List<ModifyStatStackState>(maxStacks);
            bySkill[sourceAbilityName] = stacks;
            if (!hasShown)
            {
                heroControl.RefreshObservers(type, (int)modifyValue);
                hasShown = true;
            }
        }

        // per-skill cap reached => refresh duration (max) and exit (no re-apply)
        if (stacks.Count >= maxStacks)
        {
            var s = stacks[0];
            s.remainingTurn = Mathf.Max(s.remainingTurn, remainingTurn);
            stacks[0] = s;
            if (!hasShown)
            {
                heroControl.RefreshObservers(type, (int)modifyValue);
                hasShown = true;
            }
            SyncModifyStatDebug();
            return;
        }

        stacks.Add(new ModifyStatStackState(remainingTurn, modifyValue, attacker));
        if (!hasShown)
        {
            heroControl.RefreshObservers(type, (int)modifyValue);
            hasShown = true;
        }
        SyncModifyStatDebug();
    }

    // Backward-compatible overload (giữ nguyên signature cũ)
    //public void ApplyModifyStat(string sourceAbilityName, ModifyStatType type, int remainingTurn, float modifyValue, int maxStacks, bool instant = true)
    //{
    //    int fallbackAttackerId = -1;
    //    ApplyModifyStat(sourceAbilityName, type, remainingTurn, modifyValue, maxStacks, fallbackAttackerId, instant);
    //}

    public void ApplyStatOnStartBattle(ModifyStatType type, int value)
    {
        // cộng dồn nếu đã có key, còn không thì add mới
        if (dicOnStartBattle.TryGetValue(type, out int current))
            dicOnStartBattle[type] = current + value;
        else
            dicOnStartBattle.Add(type, value);

        SyncOnStartBattleDebug();
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
    [SerializeField] private float currentShield;
    public float CurrentShield { get => currentShield; set => currentShield = Mathf.Max(0f, value); }
    public float maxShield;
    [SerializeField] private float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Max(0f, value); }

    [SerializeField] private float currentMana;
    public float CurrentMana { get => currentMana; set => currentMana = Mathf.Max(0f, value); }

    public float MaxHealth
    {
        get => finalStat?.health ?? 0f;
        set => finalStat.health = value;
    }
    public float MaxMana => 1000f;

    public float Damage
    {
        get => finalStat?.damage ?? 0f;
        set => finalStat.damage = value;
    }
    public float Armor
    {
        get => finalStat?.armor ?? 0f;
        set => finalStat.armor = value;
    }

    public float CritRate
    {
        get => finalStat?.critRate ?? 0f;
        set => finalStat.critRate = value;
        }
    public float CritDamage
    {
        get => finalStat?.critDamage ?? 0f;
        set => finalStat.critDamage = value;
    }
    public float LifeSteal
    {
        get => finalStat?.lifeSteal ?? 0f;
        set => finalStat.lifeSteal = value;
    }
    public float ControlFree
    {
        get => finalStat?.controlFree ?? 0f;
        set => finalStat.controlFree = value;
    }
    public float Speed => finalStat != null ? finalStat.speed : 0f;
    public float skillChanceFinal;
    private void Awake()
    {
        if (heroControl == null)
            heroControl = GetComponent<HeroControl>();
        baseInfo = heroControl != null ? heroControl.HeroInfo : null;
        dicOnStartBattle.Clear();
    }
    public void Init()
    {

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
                lifeSteal = baseInfo.lifeSteal,
                controlFree = baseInfo.controlFree,
                speed = baseInfo.speed,
                power = 0f
            };

        }
        skillChanceFinal = baseInfo.skillChance;
        currentHealth = finalStat.health;
        if (heroControl.HeroInfo.ID == 51)
        {
            maxShield = finalStat.health * 0.5f;
            currentShield = maxShield;
        }
        currentMana = 0f;
    }
    public void GainStatByEmpower()// tăng chỉ số dựa vào skill 3 sao
    {
        AbilityInfo empowerInfo = heroControl.HeroInfo.empower;
        if (empowerInfo == null) return;
        if(empowerInfo.effects == null || empowerInfo.effects.Count == 0) return;
        HeroInstance heroInstance = PlayerInventory.Instance.GetHeroInstance(heroControl.HeroInfo.ID);
        int levelEmpower = heroInstance.GetAbilityLevel(AbilityType.Empower);
        foreach (var effect in empowerInfo.effects)
        {
            
            if(effect.type == AbilityEffectType.ModifyStat)
            {
                ApplyStats(effect.statType, effect.modifyValue + levelEmpower * effect.valueUpPerLevel, true);
            }
        }
    }
    public void GainValueBySoul(HeroInstance instance, FightSoulInfo soulInfo)
    {
        if (soulInfo.fightSoulType == FightSoulType.ManaSoul)
        {
            if (soulInfo.soulValueConfigs == null || soulInfo.soulValueConfigs.Length == 0)
                return;
            GainMana(soulInfo.soulValueConfigs[instance.GetLevelSoul(0) - 1].value, true);
        }
        else if(soulInfo.fightSoulType == FightSoulType.SkillRateSoul)
        {
            if (soulInfo.soulValueConfigs == null || soulInfo.soulValueConfigs.Length == 0)
                return;
            skillChanceFinal += soulInfo.soulValueConfigs[instance.GetLevelSoul(1) - 1].value / 100f;
        }
        else if(soulInfo.fightSoulType == FightSoulType.ControlFreeSoul)
        {
            if (soulInfo.soulValueConfigs == null || soulInfo.soulValueConfigs.Length == 0)
                return;
            ControlFree += soulInfo.soulValueConfigs[instance.GetLevelSoul(2) - 1].value;
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
            case ModifyStatType.LifeSteal:
                GainLifeSteal(value, instant);
                break;
            case ModifyStatType.CritDamage:
                break;
            case ModifyStatType.Speed:
                break;
        }
    }

    public void GainHPMax(float value, bool instant = false)
    {
        finalStat.health *= (1 + value / 100);
        currentHealth = finalStat.health;
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;
        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
    }

    public void GainArmor(float value)
    {
        finalStat.armor *= (1 + value / 100);
    }

    public void GainMaxShield(float value)
    {
        maxShield = value;
        currentShield = maxShield;
        heroControl.RefreshObservers(HeroNotifyType.ShieldChanged, currentShield / maxShield);
    }

    public void GainCritRate(float value, bool instant = false)
    {
        finalStat.critRate += value;
        if (instant) return;
    }
    public void GainLifeSteal(float value, bool instant = false)
    {
        finalStat.lifeSteal += value;
        if (instant) return;
    }

    public void GainDamage(float value)
    {
        finalStat.damage *= (1 + value / 100);
    }

    public void GainMana(int value, bool instant = false)
    {
        int finalValue = (int)GetFinalValueAfterModifyStat(ModifyStatType.ManaRecovery, value);
        currentMana += finalValue;
        if (currentMana >= 1000)
        {
            currentMana = 1000;
        }

        float mana01 = currentMana / (float)MaxMana;
        heroControl.RefreshObservers(HeroNotifyType.ManaChanged, mana01);
        if (instant) return;
        heroControl.RefreshObservers(ModifyStatType.Mana, finalValue);
    }

    public void MinusMana(int value, bool instant = false)
    {
        currentMana -= value;
        if (currentMana <= 0)
        {
            currentMana = 0;
        }
        float mana01 = currentMana / (float)MaxMana;

        heroControl.RefreshObservers(HeroNotifyType.ManaChanged, mana01);
        if (instant) return;
        heroControl.RefreshObservers(ModifyStatType.Mana, -value);
    }

    public void GainHP(int value, DamageType damageType, bool instant = false)
    {
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

    public void LifeStealHP(int value, DamageType damageType, bool instant = false)
    {
        float percentLifeSteal = GetTotalModifyPercent(ModifyStatType.LifeSteal) + LifeSteal;
        if (percentLifeSteal <= 0) return;
        int hpLifeSteal = (int)(value * (percentLifeSteal / 100f));
        if (hpLifeSteal <= 0) return;
        CurrentHealth += hpLifeSteal;
        if (currentHealth >= MaxHealth)
        {
            currentHealth = MaxHealth;
        }
        if (instant) return;
        float health01 = CurrentHealth / (float)MaxHealth;

        heroControl.RefreshObservers(HeroNotifyType.HPChanged, health01);
        heroControl.RefreshObservers(HPNotifyType.HPPlus, damageType, hpLifeSteal);
    }

    public void MinusHP(int value, DamageType damageType, bool instant = false)
    {
        if (heroControl.CanDodge) return;
        bool shieldJustBroke = false;
        if (currentShield > 0)
        {
            currentShield -= value;
            if (currentShield <= 0f)
            {
                currentShield = 0f;
                shieldJustBroke = true;//cho hero 51
            }
            if (!instant)
            {
                float shield01 = CurrentShield / (float)maxShield;
                heroControl.RefreshObservers(HeroNotifyType.ShieldChanged, shield01);
            }


        }
        else
        {
            CurrentHealth -= value;
        }
        // NEW: khi shield vừa vỡ => apply Burn cho toàn bộ enemy
        if (shieldJustBroke && BattlefieldRegistry.Instance != null)
        {
            string enemyTeam = heroControl.CompareTag("Hero") ? "Enemy" : "Hero";
            var enemies = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);

            for (int i = 0; i < enemies.Count; i++)
            {
                var root = enemies[i];
                if (root == null) continue;

                var enemyControl = root.GetComponent<HeroControl>();
                if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                if (enemyControl.LeftBattle) continue;

                var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
                if (recv != null && recv.IsDead) continue;

                // duration/damage/maxStacks: chỉnh các giá trị này theo design của bạn
                List<AbilityEffect> effectsSpecial = heroControl.HeroInfo.passive.GetEffectsOnSpecial();
                if(effectsSpecial == null || effectsSpecial.Count == 0) continue;
                float damagePerTurn = maxShield * effectsSpecial[0].modifyValue;
                

                enemyControl.HeroStatRuntime.ApplyAES(
                    heroControl.HeroInfo.passive.abilityName,
                    AbilityEffectType.Burn,
                    effectsSpecial[0].durationTurn,
                    (int)damagePerTurn,
                    effectsSpecial[0].stackCount,
                    heroControl
                );
            }
        }
        if (heroControl.HeroInfo.ID == 57 && currentHealth > 0 && currentHealth / MaxHealth < 0.2f
            && heroControl.HeroDodge.CountDodge < 1)
        {
            heroControl.SetCanDodge();
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

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
            case AbilityEffectType.Stun:
                ApplyStun();
                break;
            case AbilityEffectType.Poison:
                ApplyPoison();
                break;
            case AbilityEffectType.Bleeding:
                ApplyBleeding();
                break;
            case AbilityEffectType.Charge:
                ApplyCharge();
                break;
            case AbilityEffectType.Paralysis:
                ApplyParalyze();
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
    void ApplyPoison()
    {
        heroControl.SpriteEffect.color = new Color(
            0 / 255f,
            255f / 255f,
            0f / 255f,
            150f / 255f
        );
        ClearOldEffect(AbilityEffectType.Poison);
        GameObject posionEffect = EffectManager.Instance.Spawn(
            AbilityEffectType.Poison,
            heroControl.SpriteEffect.transform
        );

        Transform effectTransform = posionEffect.transform;
        Vector3 heroScale = heroControl.transform.localScale;
        Vector3 effectScale = effectTransform.localScale;

        effectTransform.localScale = new Vector3(
            effectScale.x / heroScale.x,
            effectScale.y / heroScale.y,
            1
        );
        if (heroControl.HeroInfo.UIForBattleSO != null)
        {
            Vector3 effectPos = heroControl.HeroInfo.UIForBattleSO.GetPosition(AbilityEffectType.Poison);
            effectTransform.localPosition = effectPos;
        }
        heroControl.SpriteEffect.gameObject.SetActive(true);
    }
    void ApplyBleeding()
    {
        heroControl.SpriteEffect.color = new Color(
            255f / 255f,
            255f / 255f,
            255f / 255f,
            150f / 255f
        );
        ClearOldEffect(AbilityEffectType.Bleeding);
        GameObject bleedingEffect = EffectManager.Instance.Spawn(
            AbilityEffectType.Bleeding,
            heroControl.SpriteEffect.transform
        );

        Transform effectTransform = bleedingEffect.transform;
        Vector3 heroScale = heroControl.transform.localScale;
        Vector3 effectScale = effectTransform.localScale;

        effectTransform.localScale = new Vector3(
            effectScale.x / heroScale.x,
            effectScale.y / heroScale.y,
            1
        );
        if (heroControl.HeroInfo.UIForBattleSO != null)
        {
            Vector3 effectPos = heroControl.HeroInfo.UIForBattleSO.GetPosition(AbilityEffectType.Bleeding);
            effectTransform.localPosition = effectPos;
        }
        heroControl.SpriteEffect.gameObject.SetActive(true);
    }

    void ApplyStun()
    {
        ClearOldEffect(AbilityEffectType.Stun);

        GameObject stunEffect = EffectManager.Instance.Spawn(
            AbilityEffectType.Stun,
            heroControl.transform
        );
        if (stunEffect != null)
        {

            Transform effectTransform = stunEffect.transform;
            Vector3 heroScale = heroControl.transform.localScale;
            Vector3 effectScale = effectTransform.localScale;

            effectTransform.localScale = new Vector3(
                effectScale.x / heroScale.x,
                effectScale.y / heroScale.y,
                1
            );
            if (heroControl.HeroInfo.UIForBattleSO != null)
            {
                Vector3 effectPos = heroControl.HeroInfo.UIForBattleSO.GetPosition(AbilityEffectType.Stun);
                effectTransform.localPosition = effectPos;
            }

        }
    }
   

    public void ApplyUnleashChargeEffect(int damageUnleash)
    {
        if (HasAES(AbilityEffectType.Charge))
        {
            ClearOldEffect(AbilityEffectType.Charge);
            heroControl.HeroReceiveDamagee.ReceiveDamage(damageUnleash, DamageType.normalDamage, true, true);
        }
    }

    void ApplyCharge()
    {
        ClearOldEffect(AbilityEffectType.Charge);

        GameObject chargeEffect = EffectManager.Instance.Spawn(
            AbilityEffectType.Charge,
            heroControl.transform
        );

        if (chargeEffect != null)
        {

            Transform effectTransform = chargeEffect.transform;
            Vector3 heroScale = heroControl.transform.localScale;
            Vector3 effectScale = effectTransform.localScale;

            effectTransform.localScale = new Vector3(
                effectScale.x / heroScale.x,
                effectScale.y / heroScale.y,
                1
            );

            Vector3 effectPos = effectTransform.position;
            effectTransform.localPosition = new Vector3(
                0,
                0.25f,
                -0.1f
            );
        }
    }

    void ApplyParalyze()
    {
        ClearOldEffect(AbilityEffectType.Paralysis);

        GameObject paralysisEffect = EffectManager.Instance.Spawn(
            AbilityEffectType.Paralysis,
            heroControl.transform
        );

        if (paralysisEffect != null)
        {

            Transform effectTransform = paralysisEffect.transform;
            Vector3 heroScale = heroControl.transform.localScale;
            Vector3 effectScale = effectTransform.localScale;

            effectTransform.localScale = new Vector3(
                effectScale.x / heroScale.x,
                effectScale.y / heroScale.y,
                1
            );

            Vector3 effectPos = paralysisEffect.transform.position;
            effectTransform.localPosition = new Vector3(
                0,
                0.25f,
                -0.1f
            );
        }
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
        ClearOldEffect(AbilityEffectType.Burn);
        heroControl.SpriteEffect.gameObject.SetActive(false);
    }
    void CancelPoison()
    {
        ClearOldEffect(AbilityEffectType.Poison);
        heroControl.SpriteEffect.gameObject.SetActive(false);
    }
    void CancelBleeding()
    {
        ClearOldEffect(AbilityEffectType.Bleeding);
        heroControl.SpriteEffect.gameObject.SetActive(false);
    }

    void CancelStun()
    {
        ClearOldEffectInHeroControl(AbilityEffectType.Stun);
    }

    void CancelParalysis()
    {
        ClearOldEffectInHeroControl(AbilityEffectType.Paralysis);
    }

    void ClearOldEffectInHeroControl(AbilityEffectType type)
    {
        foreach (Transform child in heroControl.transform)
        {
            Effect_Item effect_item = child.GetComponent<Effect_Item>();
            if (effect_item != null && effect_item.GetAbilityType() == type)
                Destroy(child.gameObject);
        }
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

    public float GetFinalValueAfterModifyStat(ModifyStatType type, float baseValue)
    {
        float totalPercent = GetTotalModifyPercent(type);
        switch (type)
        {
            case ModifyStatType.CritRate:
                totalPercent += CritRate;
                break;
            case ModifyStatType.LifeSteal:
                totalPercent += LifeSteal;
                break;
            case ModifyStatType.ControlFree:
                totalPercent += ControlFree;
                break;
        }
        return baseValue * (1f + totalPercent / 100f);
    }

    // ===================== Debug (Inspector) =====================
    [System.Serializable]
    private struct AESDebugItem
    {
        public AbilityEffectType type;
        public string sourceAbilityName;

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

        foreach (var typeKv in aesStacksByType)
        {
            var type = typeKv.Key;
            var bySkill = typeKv.Value;
            if (bySkill == null) continue;

            foreach (var skillKv in bySkill)
            {
                string skillName = skillKv.Key;
                var stacks = skillKv.Value;
                if (stacks == null) continue;

                var copy = new List<AESStackState>(stacks.Count);
                for (int i = 0; i < stacks.Count; i++)
                    copy.Add(stacks[i]);

                aesDebug.Add(new AESDebugItem
                {
                    type = type,
                    sourceAbilityName = skillName,
                    stacks = copy
                });
            }
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

    [System.Serializable]
    private struct StartBattleStatDebugItem
    {
        public ModifyStatType type;
        public int value;
    }

    [Header("Debug OnStartBattle (Inspector)")]
    [SerializeField] private List<StartBattleStatDebugItem> onStartBattleDebug = new List<StartBattleStatDebugItem>();

    private void SyncOnStartBattleDebug()
    {
        onStartBattleDebug.Clear();
        foreach (var kv in dicOnStartBattle)
        {
            onStartBattleDebug.Add(new StartBattleStatDebugItem
            {
                type = kv.Key,
                value = kv.Value
            });
        }
    }
}