using UnityEngine;

public class HeroSkillState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {
       
        hero.HeroControl.IsSkillPressed = false;

        //if (hero.heroControl.HasBeenTransform)
        //{
        //    hero.heroControl.RefreshObservers((heroControl.SkillObserver, hero.heroControl.heroInfo.durationTransformSkill3));
        //}
        //else
        //{
        //    hero.heroControl.RefreshObservers((heroControl.SkillObserver, hero.heroControl.heroInfo.durationSkill));
        //}
        
        //if (hero.heroControl.HasBeenTransform)
        //{
        //    hero.heroControl.ChangeAnimationState(heroStateManager.hero_T_Skill);
        //}
        //else
        //{
        //    hero.heroControl.ChangeAnimationState(heroStateManager.hero_Skill);
        //}
           
    }

    public override void ExitState(HeroStateManager hero)
    {
        
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
       
    }

    public override void UpdateState(HeroStateManager hero)
    {
       
        if ((hero.HeroControl.CheckCurrentAnimation(HeroStateManager.hero_Skill, 0.9f, 1)))
        {

            //hero.heroControl.heroSkilll.ResetMana();
           
            hero.SwitchState(hero.idleState);
        }

    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
