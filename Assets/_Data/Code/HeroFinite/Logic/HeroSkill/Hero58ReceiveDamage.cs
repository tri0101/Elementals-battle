using System.Linq.Expressions;
using UnityEngine;

public class Hero58ReceiveDamage : HeroReceiveDamagee
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
    [SerializeField] bool isDiabolicPact = false;
    public  bool IsDiabolicPact
    {
        set
        {
            isDiabolicPact = value;
            
        }
         get => isDiabolicPact;
    }
    [SerializeField] HeroControl attacker;
    
    protected override void HandleDead(HeroControl attacker)
    {
        if (hasBeenUsed) base.HandleDead();
        else
        {
            isDiabolicPact = true;
            ApplyEffectForHero();
            if(this.attacker == null)
            {
                this.attacker = attacker;
                attacker.HeroReceiveDamagee.CallSpawnEffectHero("EffectHero58", new Vector3(0, 0.1f, -0.1f));
                attacker.HeroReceiveDamagee.CallSpawnEffectHero("EffectHero58_2", new Vector3(0, 0.372f, 0.1f));
                attacker.HeroReceiveDamagee.CallSpawnEffectHero("EffectHero58_3", new Vector3(0, 0.355f, -0.1f));
            }
                

        }
    }
    public void ClearEffect()
    {
        base.ClearEffectByName("EffectHero58");
        base.ClearEffectByName("EffectHero58_2");
        base.ClearEffectByName("EffectHero58_3");
        attacker.HeroReceiveDamagee.ClearEffectByName("EffectHero58");
        attacker.HeroReceiveDamagee.ClearEffectByName("EffectHero58_2");
        attacker.HeroReceiveDamagee.ClearEffectByName("EffectHero58_3");

    }
    public override float ReceiveDamage(float damage, DamageType damageType, bool shouldTakeHit, bool canDead, HeroControl attacker = null)
    {
        
        float dmg = base.ReceiveDamage(damage, damageType, shouldTakeHit, canDead, attacker);
        if(hasBeenUsed)
        {
            return dmg;
        }
        if (attacker != this.attacker && this.attacker != null)
        {
            this.attacker.HeroReceiveDamagee.ReceiveDamage(dmg, DamageType.normalDamage, true, true);
        }
        Debug.Log("Damage received: " + dmg);
        return dmg;

    }
    protected override void ApplyEffectForHero()
    {
        CallSpawnEffectHero("EffectHero58", new Vector3(0,0.1f, -0.1f));
        CallSpawnEffectHero("EffectHero58_2", new Vector3(0, 0.372f, 0.1f));
        CallSpawnEffectHero("EffectHero58_3", new Vector3(0, 0.355f, -0.1f));
    }
    
}
