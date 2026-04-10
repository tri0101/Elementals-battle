using UnityEngine;

public class HeroTransformState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {

        hero.HeroControl.IsTransform = false;
        hero.HeroControl.IsDead = false;
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Transform);

    }

    public override void ExitState(HeroStateManager hero)
    {

    }

    public override void FixedUpdateState(HeroStateManager hero)
    {

    }

    public override void UpdateState(HeroStateManager hero)
    {

        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Transform, 1f, 1)))
        {


            hero.HeroControl.LeftBattle = false;
            hero.SwitchState(hero.idleState);
        }

    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
