public static class HeroStatCalculator
{
    // =========================
    // BONUS CALCULATION
    // =========================
    public static float CalculateTotalBonus(
        HeroInstance instance,
        HeroGrowthConfig growth
    )
    {
        float bonus = 0f;

        // Level bonus
        bonus += instance.level * growth.levelBonusPerLevel;
        bonus += instance.GetAbilityLevel(AbilityType.Skill) * growth.levelBonusPerSkill;
        bonus += instance.GetAbilityLevel(AbilityType.Ultimate) * growth.levelBonusPerSkill;
        bonus += instance.GetAbilityLevel(AbilityType.Passive) * growth.levelBonusPerSkill;
        // Star bonus
        if (instance.star < growth.starBonus.Length)
            bonus += growth.starBonus[instance.star];

        // Rank bonus
        if (instance.rank < growth.rankBonus.Length)
            bonus += growth.rankBonus[instance.rank];

        return bonus;
    }

    // =========================
    // STAT CALCULATIONS
    // =========================
    public static float CalculateDamage(
        HeroInfo info,
        HeroInstance instance,
        HeroGrowthConfig growth
    )
    {
        float bonus = CalculateTotalBonus(instance, growth);
        var roleScale = RoleScaleTable.Get(info.role);

        return info.damage * roleScale.damage * (1f + bonus);
    }

    public static float CalculateHealth(
        HeroInfo info,
        HeroInstance instance,
        HeroGrowthConfig growth
    )
    {
        float bonus = CalculateTotalBonus(instance, growth);
        var roleScale = RoleScaleTable.Get(info.role);

        return info.health * roleScale.health * (1f + bonus);
    }

    public static float CalculateArmor(
        HeroInfo info,
        HeroInstance instance,
        HeroGrowthConfig growth
    )
    {
        float bonus = CalculateTotalBonus(instance, growth);
        var roleScale = RoleScaleTable.Get(info.role);

        return info.armor * roleScale.armor * (1f + bonus);
    }

    public static float CalculateSpeed(HeroInfo info)
    {
        var roleScale = RoleScaleTable.Get(info.role);
        return info.speed * roleScale.speed;
    }

    public static float CalculateCritRate(HeroInfo info)
    {
        return info.criticalRate;
    }

    public static float CalculateCritDamage(HeroInfo info)
    {
        return info.criticalDamageRate;
    }

    // =========================
    // POWER CALCULATION
    // =========================
    public static float CalculatePower(HeroStat stat)
    {
        return
            stat.damage * 1.0f +
            stat.health * 0.2f +
            stat.armor * 0.6f +
            stat.critRate * 50f +
            stat.critDamage * 30f +
            stat.speed * 10f;
    }

    // =========================
    // FULL STAT BUILD
    // =========================
    public static HeroStat Calculate(
        HeroInfo info,
        HeroInstance instance,
        HeroGrowthConfig growth
    )
    {
        HeroStat stat = new HeroStat
        {
            damage = CalculateDamage(info, instance, growth),
            health = CalculateHealth(info, instance, growth),
            armor = CalculateArmor(info, instance, growth),
            critRate = CalculateCritRate(info),
            critDamage = CalculateCritDamage(info),
            speed = CalculateSpeed(info)
        };
        
        stat.power = CalculatePower(stat);
       
        return stat;
    }

    public static void ApplyStartBattlePassive(
    HeroInfo info,
    HeroInstance instance,
    HeroGrowthConfig growth,
    ref HeroStat stat
)
    {
        if (info.passive == null || info.passive.effects == null)
            return;

        foreach (var effect in info.passive.effects)
        {
            // 1. chỉ xử lý ModifyStat
            if (effect.type != AbilityEffectType.ModifyStat)
                continue;

            // 2. chỉ gọi khi StartBattle
            if (effect.timeToCall != TimesToCall.onStartBattle)
                continue;

            // 3. roll chance
            if (UnityEngine.Random.value > effect.chance)
                continue;

            // 4. apply stat
            ApplyModifyStat(effect, ref stat);
        }
    }
    private static void ApplyModifyStat(
    AbilityEffect effect,
    ref HeroStat stat
)
    {
        float percent = effect.modifyValue;

        switch (effect.statType)
        {
            case ModifyStatType.Damage:
                stat.damage *= (1f + percent);
                break;

            case ModifyStatType.Health:
                stat.health *= (1f + percent);
                break;

            case ModifyStatType.Speed:
                stat.speed *= (1f + percent);
                break;

            case ModifyStatType.Armor:
                stat.armor *= (1f + percent);
                break;
        }

        // Nếu durationTurn != -1
        // => đây là buff có thời hạn
        // có thể thêm vào hệ thống BuffManager sau
    }
}
