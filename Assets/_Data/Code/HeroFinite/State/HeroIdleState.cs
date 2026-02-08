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
        
     
        if (hero.HeroControl.IsAttack && !hero.HeroControl.IsPrepare)
        {
            hero.SwitchState(hero.runState);
            return;
        }
        
        if (hero.HeroControl.IsSkill && !hero.HeroControl.IsPrepare)
        {
            hero.SwitchState(hero.runState);
            return;
        }
        if (hero.HeroControl.IsUltimate && !hero.HeroControl.IsPrepare)
        {
            hero.SwitchState(hero.runState);
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
