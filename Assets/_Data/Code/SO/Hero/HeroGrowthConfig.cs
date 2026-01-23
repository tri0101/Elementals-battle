using UnityEngine;

[CreateAssetMenu(menuName = "hero/Hero Growth Config")]
public class HeroGrowthConfig : ScriptableObject
{
    [Header("Level Bonus (%)")]
    public float levelBonusPerLevel = 0.02f; // mỗi level +2%

    [Header("Star Bonus (%)")]
    public float[] starBonus;
    // index = star
    // ví dụ: [0, 0.15, 0.35, 0.6, 0.9, 1.25, 1.65]

    [Header("Rank Bonus (%)")]
    public float[] rankBonus;
    // ví dụ: [0, 0.1, 0.25, 0.45, 0.7]
}
