using System.Linq.Expressions;
using UnityEngine;

public class HeroDesertReceiveDamage : HeroReceiveDamagee
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
        base.HandleDead(attacker);
    }
    public void ClearEffect()
    {
        

    }
    public override float ReceiveDamage(float damage, DamageType damageType, bool shouldTakeHit, bool canDead, HeroControl attacker = null)
    {

        float dmg = base.ReceiveDamage(damage, damageType, shouldTakeHit, canDead, attacker);
        if(HeroControl.HeroStatRuntime.CurrentHealth <= HeroControl.HeroStatRuntime.MaxHealth * 0.5f && !hasBeenUsed)
        {
            hasBeenUsed = true;
            SetInsane();
            heroControl.CanSkill = true;
            HeroControl.RefreshObservers("Frenzy Activated");
        }
        return dmg;

    }
    void SetInsane()
    {
        heroControl.IsCrit = true;
        heroControl.HeroStatRuntime.skillChanceFinal = 1f;
        heroControl.HeroStatRuntime.CritRate = 99f;
        heroControl.HeroStatRuntime.CritDamage = 150f;
        heroControl.HeroStatRuntime.Damage *= 2f;
        heroControl.HeroStatRuntime.Armor *= 2f;
    }


}
