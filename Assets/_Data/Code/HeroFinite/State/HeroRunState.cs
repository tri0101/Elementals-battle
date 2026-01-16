using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HeroRunState : HeroBaseState
{

   
    public override void EnterState(HeroStateManager hero)
    {
       
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Run);
        
        
     
    }
    public override void ExitState(HeroStateManager hero)
    {
       
    }
    public override void UpdateState(HeroStateManager hero)
    {


        
        if (hero.HeroControl.IsAttackPressed)
        {
            hero.SwitchState(hero.attackState);
            return;
        }
        
        if (hero.HeroControl.IsSkillPressed)
        {
            hero.SwitchState(hero.skillState);
            return;
        }
     
        if (hero.HeroControl.MoveX == 0)
            hero.SwitchState(hero.idleState);

       
    }
    public override void FixedUpdateState(HeroStateManager hero)
    {
        hero.HeroControl.HeroRun.Move();
    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
