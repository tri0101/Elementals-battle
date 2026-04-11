using UnityEngine;

public class HeroUltimateState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {

      
        if (hero.HeroControl.IsUltimate)
        {
            hero.HeroControl.IsUltimate = false;
            hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Ultimate);

        }
        else
        {
            hero.HeroControl.IsUltimateSpecial = false;
            hero.HeroControl.CanUltimateSpecial = false;
            hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Ultimate_Special);
        }

        hero.HeroControl.HeroUltimate.ResetMana();
        hero.HeroControl.HeroUltimate.RefreshTotalDmg();
    }

    public override void ExitState(HeroStateManager hero)
    {
        hero.HeroControl.HeroEventt.NotifyCanHideTotalDmg();
        if (hero.HeroControl.HeroInfo.ID == 4)
        {
            if (hero.HeroControl.CheckEnemyDead(1))
            {
                hero.HeroControl.HeroEventt.SetEffect();
            }
        }
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {

    }

    public override void UpdateState(HeroStateManager hero)
    {

        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Ultimate, 0.98f, 1))
            || hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Ultimate_Special, 0.98f,1))
        {
           

            hero.HeroControl.GoBackBattleTarget();

            hero.SwitchState(hero.runState);
        }


    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
