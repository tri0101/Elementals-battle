using UnityEngine;

public class HeroDeathState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {
        hero.HeroControl.IsFinished = false;
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

        
        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Dead, 1f, 1)))
        {
            //if (hero.HeroControl.HeroInfo.ID == 58) return;
            //hero.HeroControl.IsFinished = true;
            hero.HeroControl.gameObject.SetActive(false);
        }
    }
}
