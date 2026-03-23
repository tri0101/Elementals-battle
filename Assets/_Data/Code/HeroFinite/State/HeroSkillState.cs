using UnityEngine;

public class HeroSkillState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {
       
        hero.HeroControl.IsSkill = false;
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Skill);

    }

    public override void ExitState(HeroStateManager hero)
    {
        
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
       
    }

    public override void UpdateState(HeroStateManager hero)
    {

        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Skill, 1f, 1)))
        {

            
            hero.HeroControl.GoBackBattleTarget();

            hero.SwitchState(hero.runState);
        }

    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
