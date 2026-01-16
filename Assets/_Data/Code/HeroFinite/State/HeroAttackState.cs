using Unity.VisualScripting;
using UnityEngine;

public class HeroAttackState : HeroBaseState
{
    bool canCombo;

    public override void EnterState(HeroStateManager hero)
    {
        
        hero.HeroControl.IsAttackPressed = false;
        hero.HeroControl.HeroAttackk.CountAttack = 0;
        PlayAttack(hero);
    }

    public override void ExitState(HeroStateManager hero)
    {
        
    }

    public override void FixedUpdateState(HeroStateManager hero)
    {
        
    }

    public override void UpdateState(HeroStateManager hero)
    {
        AnimatorStateInfo info = hero.HeroControl.Animator.GetCurrentAnimatorStateInfo(0);
        float duration = 0f;
        switch (hero.HeroControl.HeroAttackk.CountAttack)
        {
            case 0:
                duration = hero.HeroControl.HeroInfo.durationA1;
                break;
            case 1:
                duration = hero.HeroControl.HeroInfo.durationA2;
                break;
            
        }
        if (info.normalizedTime >= duration)
            canCombo = true;
        if (canCombo && hero.HeroControl.IsAttackPressed)
        {
            hero.HeroControl.IsAttackPressed = false;
            hero.HeroControl.HeroAttackk.CountAttack++;

            if (hero.HeroControl.HeroAttackk.CountAttack <= 2)
            {
                PlayAttack(hero);
                return;
            }
        }

        if (info.normalizedTime >= 1f)
        {
            hero.SwitchState(hero.idleState);
        }
    }

    void PlayAttack(HeroStateManager hero)
    {
       
       
    }
    public override void LateUpdateState(HeroStateManager hero)
    {

    }
}
