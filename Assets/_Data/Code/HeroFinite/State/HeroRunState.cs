using UnityEngine;

public class HeroRunState : HeroBaseState
{
    public override void EnterState(HeroStateManager hero)
    {
        hero.HeroControl.IsPrepare = false;
        hero.HeroControl.ChangeAnimationState(HeroStateManager.hero_Run);
    }

    public override void ExitState(HeroStateManager hero)
    {
    }

    public override void UpdateState(HeroStateManager hero)
    {
       
        if (Vector3.Distance(
            hero.transform.position,
            hero.HeroControl.BattleTarget) < 0.1f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.HeroControl.SetArrivedBattle();
            hero.SwitchState(hero.idleState);
            return;
        }


        float dx = Mathf.Abs(
            hero.transform.position.x -
            hero.HeroControl.distanceToTarget.x
        );

        if (hero.HeroControl.IsAttack && dx <= 0.1f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.SwitchState(hero.attackState);
            return;
        }
        if (hero.HeroControl.IsSkill && dx <= 0.1f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.SwitchState(hero.skillState);
            return;
        }
        if (hero.HeroControl.IsUltimate && dx <= 0.1f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.SwitchState(hero.ultimateState);
            return;
        }
        if (hero.HeroControl.IsClear && dx <= 0.1f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.HeroControl.IsClear = false;
            hero.SwitchState(hero.idleState);
            return;
        }

   
     
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
        if (hero.HeroControl.NeedMoveToBattle)
        {
            hero.HeroControl.HeroRun.MoveTo(
                hero.HeroControl.BattleTarget,100f);
        }
        else if (hero.HeroControl.IsAttack)
        {
            hero.HeroControl.HeroRun.MoveTo(
              hero.HeroControl.distanceToTarget, hero.HeroControl.HeroInfo.normalAttack.speedToEnemy);
        }
        else if (hero.HeroControl.IsSkill)
        {
            hero.HeroControl.HeroRun.MoveTo(
                hero.HeroControl.distanceToTarget, hero.HeroControl.HeroInfo.skill.speedToEnemy);
        }
        else if (hero.HeroControl.IsUltimate)
        {
            hero.HeroControl.HeroRun.MoveTo(
                hero.HeroControl.distanceToTarget, hero.HeroControl.HeroInfo.ultimate.speedToEnemy);
        }
        else if (hero.HeroControl.IsClear)
        {
            hero.HeroControl.HeroRun.MoveTo(
                hero.HeroControl.distanceToTarget, 100f);
        }
      
    }

    public override void LateUpdateState(HeroStateManager hero)
    {
    }
}
