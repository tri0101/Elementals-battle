using TMPro;
using UnityEngine;

public class HeroTakeHitState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {
        hero.HeroControl.IsTakeHit = false;
        hero.HeroControl.ChangeAnimationAnyState(HeroStateManager.hero_Take_Hit);
        
            
        
    }

    public override void ExitState(HeroStateManager hero)
    {
        
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
       
    }

    public override void LateUpdateState(HeroStateManager hero)
    {
        
    }

    public override void UpdateState(HeroStateManager hero)
    {
        if (hero.HeroControl.HeroReceiveDamagee.IsDead)
        {
            hero.SwitchState(hero.deathState);
            return;
        }
        if (hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Take_Hit, 0.99f, 1))
        {
            Debug.Log("ve idle");
            hero.SwitchState(hero.idleState);



        }
      


    }
}
