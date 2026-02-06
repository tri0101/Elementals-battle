public static class HeroStatCalculator
{
    public static HeroStat Calculate(
        HeroInfo info,
        HeroInstance instance,
        HeroGrowthConfig growth
    )
    {
        float totalBonus = 0f;

        // Level bonus
        totalBonus += instance.level * growth.levelBonusPerLevel;

        // Star bonus
        if (instance.star < growth.starBonus.Length)
            totalBonus += growth.starBonus[instance.star];

        // Rank bonus
        if (instance.rank < growth.rankBonus.Length)
            totalBonus += growth.rankBonus[instance.rank];

        // Role scale
        var roleScale = RoleScaleTable.Get(info.role);

      
        float damage = info.damage * roleScale.damage * (1 + totalBonus);
        float health = info.health * roleScale.health * (1 + totalBonus);
        float armor = info.armor * roleScale.armor * (1 + totalBonus);
        float critRate = info.criticalRate;
        float critDamage = info.criticalDamageRate;
        float speed = info.speed * roleScale.speed;

       
        float power =
            damage * 1.0f +
            health * 0.2f +
            armor * 0.6f +
            critRate * 50f +
            critDamage * 30f +
            speed * 10f;

        return new HeroStat
        {
            damage = damage,
            health = health,
            armor = armor,
            critRate = critRate,
            critDamage = critDamage,
            speed = speed,
            power = power
        };
    }
}