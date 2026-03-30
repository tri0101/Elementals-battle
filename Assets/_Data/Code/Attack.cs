using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : Subject
{
    [SerializeField] private AttackInfo attackInfo;
    public AttackInfo AttackInfo => attackInfo;
    [SerializeField] private HeroControl heroControl;
    [SerializeField] private float attackDamage;
    public float AttackDamage => attackDamage;

    [SerializeField] string myTag;
    
    private void Awake()
    {

       
        heroControl = transform.parent.parent.parent.parent.GetComponent<HeroControl>();
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageType damageType;
        if (!other.CompareTag("ReceiveField"))
        {
            Debug.Log(other.tag);
            return;
        }
        attackDamage = heroControl.HeroStatRuntime.Damage;
        attackDamage = heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(ModifyStatType.Damage, attackDamage);
        if (heroControl.IsCrit)
        {
            attackDamage *= (heroControl.HeroInfo.criticalDamageRate / 100);
            damageType = DamageType.critDamage;
        }
        else
        {
            damageType = DamageType.normalDamage;
        }
        

        Debug.Log("Attack Damage: " + attackDamage);
        var hero = other.GetComponent<HeroReceiveDamagee>();
        if (hero == null)
        {
            Debug.LogError("HeroReceiveDamagee component not found on the collided object.");
            return;
        }


        if (!heroControl.enemyTarget.Contains(hero.transform.parent.parent))
        {
            Debug.Log("Not enemy target");
            return;
        }
        hero.ReceiveDamage(attackDamage, damageType, true, false);
        if(heroControl.CurrentStringState == HeroStateManager.hero_Attack_1)
            ApplyEffectUINormal();
        else if(heroControl.CurrentStringState == HeroStateManager.hero_Skill)
            ApplyEffectUISkill();
        else if(heroControl.CurrentStringState == HeroStateManager.hero_Ultimate)
            ApplyEffectUltimate();
        NotifyObservers(this);
    }
    void ApplyEffectUltimate()
    {
        if (heroControl.CurrentStringState != HeroStateManager.hero_Ultimate)
            return;
        if (heroControl.HeroInfo == null)
        {
            Debug.LogError("HeroInfo NULL", this);
            return;
        }

        if (heroControl.HeroInfo.ultimate == null)
        {
            Debug.LogError("HeroInfo.ultimate NULL", this);
            return;
        }

        List<AbilityEffect> effects = heroControl.HeroInfo.ultimate.GetEffectsOnAttack();
        ApplyEffect(effects);
    }
    void ApplyEffectUISkill()
    {
        if (heroControl.CurrentStringState != HeroStateManager.hero_Skill)
            return;
        if (heroControl.HeroInfo == null)
        {
            Debug.LogError("HeroInfo NULL", this);
            return;
        }

        if (heroControl.HeroInfo.skill == null)
        {
            Debug.LogError("HeroInfo.skill NULL", this);
            return;
        }

        List<AbilityEffect> effects = heroControl.HeroInfo.skill.GetEffectsOnAttack();
       ApplyEffect(effects);
    }
    void ApplyEffectUINormal()
    {
        if (heroControl.CurrentStringState != HeroStateManager.hero_Attack_1)
            return;
        if (heroControl.HeroInfo == null)
        {
            Debug.LogError("HeroInfo NULL", this);
            return;
        }

        if (heroControl.HeroInfo.normalAttack == null)
        {
            Debug.LogError("HeroInfo.normal NULL", this);
            return;
        }

        List<AbilityEffect> effects = heroControl.HeroInfo.normalAttack.GetEffectsOnAttack();
        ApplyEffect(effects);
    }
    void ApplyEffect(List<AbilityEffect> listEffect)
    {
        List<AbilityEffect> effects = listEffect;


        if (effects == null || effects.Count == 0)
        {
            Debug.LogWarning("No effects returned by GetEffectsOnAttack()", this);
            return;
        }

        foreach (var effect in effects)
        {

            float chance = Mathf.Clamp01(effect.chance);
            if (chance <= 0f) continue;
            if (chance < 1f && Random.value > chance) continue;
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                string skillName = heroControl.HeroInfo.ultimate.abilityName;
                if (effect.target == AbilityTarget.CurrentTarget)
                {
                    List<Transform> targets = heroControl.enemyTarget;
                    for (int j = 0; j < targets.Count; j++)
                    {
                        var targetUnit = targets[j].GetComponent<HeroControl>();
                        if (targetUnit == null) continue;
                        int duration = heroControl.ShouldPlus ? effect.durationTurn + 1 : effect.durationTurn;
                        targetUnit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, effect.durationTurn, effect.modifyValue, effect.stackCount);

                    }

                    heroControl.HeroEventt.SetEffectToEnemy(TimesToCall.onAttack);


                }
            }
            else
            {
                foreach (var target in heroControl.enemyTarget)
                {
                    HeroControl enemyControl = target.GetComponent<HeroControl>();
                    if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                    int damagePerTurn = (int)(heroControl.HeroStatRuntime.Damage * (effect.modifyValue / 100f));
                    enemyControl.HeroStatRuntime.ApplyAES(effect.type, effect.durationTurn, damagePerTurn, effect.stackCount);
                }
            }
               
        }
    }

}
