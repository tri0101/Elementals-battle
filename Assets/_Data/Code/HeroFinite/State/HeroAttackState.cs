using Unity.VisualScripting;
using UnityEngine;

public class HeroAttackState : HeroBaseState
{
    bool canCombo;

    public override void EnterState(HeroStateManager hero)
    {
        
        hero.HeroControl.IsAttack = false;
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Attack_1);
    }

    public override void ExitState(HeroStateManager hero)
    {
        
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
        
    }

    public override void UpdateState(HeroStateManager hero)
    {
        

        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Attack_1, 0.9f, 1)))
        {

            hero.HeroControl.GoBackBattleTarget();
            hero.SwitchState(hero.runState);
        }
    }

    void PlayAttack(HeroStateManager hero)
    {
       
       
    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
