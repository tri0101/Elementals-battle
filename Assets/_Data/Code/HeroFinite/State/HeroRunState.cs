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
            hero.HeroControl.BattleTarget) < 0.05f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.HeroControl.SetArrivedBattle();
            hero.SwitchState(hero.idleState);
            return;
        }


                float dx = Mathf.Abs(
            hero.transform.position.x -
            hero.HeroControl.enemyTarget[0].position.x
        );

        if (hero.HeroControl.IsAttack && dx <= 4f)
        {
            hero.HeroControl.HeroRun.FaceDefaultDirection();
            hero.SwitchState(hero.attackState);
            return;
        }

        //if (hero.HeroControl.IsAttack)
        //{
        //    hero.SwitchState(hero.attackState);
        //    return;
        //}

        if (hero.HeroControl.IsSkill)
        {
            hero.SwitchState(hero.skillState);
            return;
        }
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
        if (hero.HeroControl.NeedMoveToBattle)
        {
            hero.HeroControl.HeroRun.MoveTo(
                hero.HeroControl.BattleTarget);
        }
        else if (hero.HeroControl.IsAttack)
        {
            hero.HeroControl.HeroRun.MoveTo(
                hero.HeroControl.enemyTarget[0].position);
        }
      
    }

    public override void LateUpdateState(HeroStateManager hero)
    {
    }
}
