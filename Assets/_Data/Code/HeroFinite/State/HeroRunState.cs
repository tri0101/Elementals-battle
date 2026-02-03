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

        if (Vector3.Distance(
            hero.transform.position,
            hero.HeroControl.BattleTarget) < 0.05f)
        {
            
            hero.HeroControl.SetArrivedBattle();
            hero.SwitchState(hero.idleState);
            return;
        }
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
     

 
    }
    public override void FixedUpdateState(HeroStateManager hero)
    {
        
        if (hero.HeroControl.NeedMoveToBattle)
        {
            hero.HeroControl.HeroRun.MoveTo(hero.HeroControl.BattleTarget);
        }
        
    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
