using Unity.VisualScripting;
using UnityEngine;

public class HeroDodgeState : HeroBaseState
{
   

    public override void EnterState(HeroStateManager hero)
    {

        hero.HeroControl.IsDodge = false;
        //hero.HeroControl.LeftBattle = true;
        hero.HeroControl.HeroDodge.IncreaseDodgeCount();
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Dodge);
    }

    public override void ExitState(HeroStateManager hero)
    {
       
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {

    }

    public override void UpdateState(HeroStateManager hero)
    {


        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Dodge, 1f, 1)))
        {

            hero.HeroControl.GoBackBattleTarget();

            hero.SwitchState(hero.idleState);
        }
    }

    void PlayAttack(HeroStateManager hero)
    {


    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
