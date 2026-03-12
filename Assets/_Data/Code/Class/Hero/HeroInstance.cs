using NUnit.Framework;
using System.Collections.Generic;
[System.Serializable]
public class HeroInstance
{
    public int heroId;
    public int level;
    public int currentExp;
    public int speedLevel;
    public int currentSpeedExp;
    public int star;
    public int rank;
    public List<SkillInstance> skillInstances = new List<SkillInstance>();
    public List<FightSoulInstance> soulsInstances = new List<FightSoulInstance>();
    public int shard;

    public void InitSkillInstances()
    {
        skillInstances.Add(new SkillInstance { AbilityType = AbilityType.Skill, level = 1 });
        skillInstances.Add(new SkillInstance { AbilityType = AbilityType.Ultimate, level = 1 });
        skillInstances.Add(new SkillInstance { AbilityType = AbilityType.Passive, level = 1 });
    }
    
    public void AddFightSoul(int id)
    {
        soulsInstances.Add(new FightSoulInstance {soulID =(id), level = 1, currentExp = 0 });
    }
    
    public int GetAbilityLevel(AbilityType type)
    {
        var skill = skillInstances.Find(s => s.AbilityType == type);

        if (skill == null)
            return 0;

        return skill.level;
    }
    
}
