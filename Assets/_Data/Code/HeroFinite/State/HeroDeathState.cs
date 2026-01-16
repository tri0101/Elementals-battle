using UnityEngine;

public class HeroDeathState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {
        hero.HeroControl.ChangeLayerAtReceive("RollLayer");
        hero.HeroControl.ChangeAnimationAnyState(HeroStateManager.hero_Dead);
        
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
    }
}
