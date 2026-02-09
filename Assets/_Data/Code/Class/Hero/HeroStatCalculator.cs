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
}
