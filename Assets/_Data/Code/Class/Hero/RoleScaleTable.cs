public struct RoleScale
{
    public float damage;
    public float health;
    public float armor;
    public float speed;
}

public static class RoleScaleTable
{
    public static RoleScale Get(RoleHero role)
    {
        switch (role)
        {
            // Tank: ít damage, nhiều HP, cao armor
            case RoleHero.Tank:
                return new RoleScale
                {
                    damage = 0.7f,
                    health = 0.8f,
                    armor = 1.1f,
                    speed = 1f
                };

            // DPS: damage cao, máu thấp, thủ yếu
            case RoleHero.DPS:
                return new RoleScale
                {
                    damage = 1.6f,
                    health = 0.8f,
                    armor = 0.7f,
                    speed = 1f
                };

            // Support: cân bằng giữa damage / health / armor, 
            case RoleHero.Support:
                return new RoleScale
                {
                    damage = 1.0f,
                    health = 1.1f,
                    armor = 1.1f,
                    speed = 1f
                };

            default:
                return new RoleScale
                {
                    damage = 1f,
                    health = 1f,
                    armor = 1f,
                    speed = 1f
                };
        }
    }
}