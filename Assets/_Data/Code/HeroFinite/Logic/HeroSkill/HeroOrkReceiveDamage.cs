using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class HeroOrkReceiveDamage : HeroReceiveDamagee
{
    

    protected override void HandleDead(HeroControl attacker)
    {
        base.HandleDead(attacker);
    }

    public void ClearEffect()
    {
    }

    public override float ReceiveDamage(float damage, DamageType damageType, bool shouldTakeHit, bool canDead, HeroControl attacker = null)
    {
        float dmg = base.ReceiveDamage(damage, damageType, shouldTakeHit, canDead, attacker);
        attacker.HeroReceiveDamagee.ReceiveDamage(dmg * 0.25f, DamageType.counterDamage, false, false, heroControl);
        return dmg;
    }

    public override void CallWaitForAttackerFinished(HeroControl attacker)
    {
        base.CallWaitForAttackerFinished(attacker);
    }


    

    public override void CallTakeHit(bool shouldTakeHit)
    {
        if (shouldTakeHit)
        {
            heroControl.SetIsTakeHit();
        }
    }

  
}