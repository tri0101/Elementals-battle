using TMPro;
using UnityEngine;

public class HeroBlockState : HeroBaseState
{
    private bool isBlocking = false;
    public override void EnterState(HeroStateManager hero)
    {
        if (isBlocking) return;
        isBlocking = true;
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Block_Start);
    }

    public override void ExitState(HeroStateManager hero)
    {
    }

    public override void UpdateState(HeroStateManager hero)
    {

        //if (hero.HeroControl.NeedMoveToBattle)
        //{
        //    hero.SwitchState(hero.runState);
        //}
        if (!hero.HeroControl.IsBlock)
        {
            hero.SwitchState(hero.idleState);
        }


    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
        
    }

    public override void LateUpdateState(HeroStateManager hero)
    {
    }
}
