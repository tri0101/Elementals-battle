using UnityEngine;

public class HeroUltimateState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {

        hero.HeroControl.IsUltimate = false;
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Ultimate);
        hero.HeroControl.HeroUltimate.ResetMana();
    }

    public override void ExitState(HeroStateManager hero)
    {

    }

    public override void FixedUpdateState(HeroStateManager hero)
    {

    }

    public override void UpdateState(HeroStateManager hero)
    {

        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Ultimate, 1f, 1)))
        {

            hero.HeroControl.GoBackBattleTarget();

            hero.SwitchState(hero.runState);
        }

    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
