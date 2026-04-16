using System.Linq.Expressions;
using UnityEngine;

public class Hero51ReceiveDamage : HeroReceiveDamagee
{
    bool hasBeenUsed = false;
    public bool HasBeenUsed
    {
        get => hasBeenUsed;
        set
        {

            hasBeenUsed = value;

        }
    }
   

    protected override void HandleDead(HeroControl attacker)
    {
        
    }
    public void ClearEffect()
    {
        

    }
    //public override float ReceiveDamage(float damage, DamageType damageType, bool shouldTakeHit, bool canDead, HeroControl attacker = null)
    //{

    //    float dmg = base.ReceiveDamage(damage, damageType, shouldTakeHit, canDead, attacker);
    //    if (hasBeenUsed)
    //    {
    //        return dmg;
    //    }
    //    if (attacker != this.attacker && this.attacker != null)
    //    {
    //        this.attacker.HeroReceiveDamagee.ReceiveDamage(dmg, DamageType.normalDamage, true, true);
    //    }
    //    Debug.Log("Damage received: " + dmg);
    //    return dmg;

    //}
   

}
