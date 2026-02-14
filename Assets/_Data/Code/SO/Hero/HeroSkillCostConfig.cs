using UnityEngine;

[CreateAssetMenu(menuName = "hero/SkillCostConfig")]
public class HeroSkillCostConfig : ScriptableObject
{
    public int[] costPerLevelSkill;
    public int[] costPerLevelUltimate;
    public int[] costPerLevelPassive;
}
