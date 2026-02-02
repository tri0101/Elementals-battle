using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HeroIdleState : HeroBaseState
{

    public override void EnterState(HeroStateManager hero)
    {

        hero.HeroControl.ChangeAnimationState((HeroStateManager.hero_Idle));

        
    }
    public override void ExitState(HeroStateManager hero)
    {
        
    }
    public override void UpdateState(HeroStateManager hero)
    {

     
        if (hero.HeroControl.IsAttackPressed)
        {
            hero.SwitchState(hero.attackState);
            return;
        }
        
        if (hero.HeroControl.IsSkillPressed)
        {
            hero.SwitchState(hero.skillState);
            return;
        }
        
        if (hero.HeroControl.MoveX != 0)
        {
            hero.SwitchState(hero.runState);
        }

    }
    public override void FixedUpdateState(HeroStateManager hero)
    {
        
    }

    public override void LateUpdateState(HeroStateManager hero)
    {
        
    }
}
