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
        
        if (hero.HeroControl.IsTakeHit)
        {
            hero.SwitchState(hero.takeHitState);
        }
        if (hero.HeroControl.IsDead)
        {
            hero.SwitchState(hero.deathState);
        }

    }
    public override void FixedUpdateState(HeroStateManager hero)
    {
        
    }

    public override void LateUpdateState(HeroStateManager hero)
    {
        
    }
}
