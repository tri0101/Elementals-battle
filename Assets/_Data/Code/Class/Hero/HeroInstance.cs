using NUnit.Framework;
using System.Collections.Generic;
[System.Serializable]
public class HeroInstance
{
    public int heroId;
    public int level;
    public int currentExp;
    public int star;
    public int rank;
    public List<SkillInstance> skillInstances = new List<SkillInstance>();
    public int shard;

    public void InitSkillInstances()
    {
        skillInstances.Add(new SkillInstance { AbilityType = AbilityType.Skill, level = 1 });
        skillInstances.Add(new SkillInstance { AbilityType = AbilityType.Ultimate, level = 1 });
        skillInstances.Add(new SkillInstance { AbilityType = AbilityType.Passive, level = 1 });
    }

    public int GetAbilityLevel(AbilityType type)
    {
        var skill = skillInstances.Find(s => s.AbilityType == type);

        if (skill == null)
            return 0;

        return skill.level;
    }
    
}
