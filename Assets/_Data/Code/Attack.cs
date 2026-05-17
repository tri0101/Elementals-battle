using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : Subject
{
    [SerializeField] private AttackInfo attackInfo;
    public AttackInfo AttackInfo => attackInfo;
    [SerializeField] private HeroControl heroControl;
    public HeroControl HeroControl
    {
        get => heroControl; set => heroControl = value;
    }
    [SerializeField] private float attackDamage;
    public float AttackDamage => attackDamage;

    [SerializeField] float finalMutipler; // chỉ gán runtime
    Transform currentTarget = null;
    private void Awake()
    {


        heroControl = transform.parent?.parent?.parent?.parent?.GetComponent<HeroControl>();


    }
    public int GetLevelBasedOnSkill()
    {
        HeroInstance heroInstance = null;
        if (heroControl.tag == "Hero")
            heroInstance = PlayerInventory.Instance.GetHeroInstance(heroControl.HeroInfo.ID);
        else
            heroInstance = BattleManager.Instance.GetEnemyInstance(heroControl.HeroInfo.ID);

        switch (heroControl.CurrentStringState)
        {
            case HeroStateManager.hero_Skill:
                return heroInstance.GetAbilityLevel(AbilityType.Skill);
            case HeroStateManager.hero_Ultimate:
                return heroInstance.GetAbilityLevel(AbilityType.Ultimate);
            default:
                return 0;
        }


    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageType damageType;

        if (!other.CompareTag("ReceiveField"))
        {
            Debug.Log(other.tag);
            return;
        }

        HeroControl otherHeroControl =
            other.transform.parent.parent.GetComponent<HeroControl>();
        var hero = other.GetComponent<HeroReceiveDamagee>();
        if (!heroControl.enemyTarget.Contains(hero.transform.parent.parent))
        {
            Debug.Log("Not enemy target");
            return;
        }
        bool isFirstAttack = false;

        if (otherHeroControl != null)
        {
            isFirstAttack = !otherHeroControl.HeroEventt.FirstAttack;

            if (isFirstAttack)
            {
                otherHeroControl.HeroEventt.FirstAttack = true;
            }
        }

        currentTarget = other.transform.parent.parent;

        attackDamage = heroControl.HeroStatRuntime.Damage;
        if(heroControl.HeroInfo.ID == 10)
        {
            if(other.transform.parent.parent.GetComponent<HeroControl>().HeroInfo.role == RoleHero.Tank)
            {
                attackDamage += attackDamage * 0.25f; // tăng 25% sát thương khi tấn công tướng địch có vai trò Tank
            }
        }
        float finalMutipler =
            attackInfo.mutiplerDamageSend + GetLevelBasedOnSkill() * 0.01f;

        this.finalMutipler = finalMutipler;

        attackDamage *= finalMutipler;

        attackDamage =
            heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
                ModifyStatType.Damage,
                attackDamage
            );

        if (heroControl.IsCrit)
        {
            attackDamage *= (heroControl.HeroStatRuntime.CritDamage / 100);
            damageType = DamageType.critDamage;
        }
        else
        {
            damageType = DamageType.normalDamage;
        }

        Debug.Log("Attack Damage: " + attackDamage);

       

        if (hero == null)
        {
            Debug.LogError("HeroReceiveDamagee component not found.");
            return;
        }

       

        float finalDamage =
            hero.ReceiveDamage(attackDamage, damageType, true, false, heroControl);

        heroControl.HeroStatRuntime.LifeStealHP(
            (int)finalDamage,
            DamageType.normalDamage
        );

        // chỉ apply effect lần đầu
        if (!isFirstAttack) return;

        if (heroControl.CurrentStringState == HeroStateManager.hero_Attack_1)
            ApplyEffectUINormal();
        else if (heroControl.CurrentStringState == HeroStateManager.hero_Skill)
            ApplyEffectUISkill();
        else if (heroControl.CurrentStringState == HeroStateManager.hero_Ultimate)
            ApplyEffectUltimate();
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
        ApplyEffect(heroControl.HeroInfo.ultimate.abilityName,effects);
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
       ApplyEffect(heroControl.HeroInfo.skill.abilityName,effects);
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
        ApplyEffect(heroControl.HeroInfo.normalAttack.abilityName,effects);
    }
    //void ApplyEffect(string nameSkill, List<AbilityEffect> listEffect)
    //{
    //    List<AbilityEffect> effects = listEffect;


    //    if (effects == null || effects.Count == 0)
    //    {
    //        Debug.LogWarning("No effects returned by GetEffectsOnAttack()", this);
    //        return;
    //    }

    //    foreach (var effect in effects)
    //    {

    //        float chance = Mathf.Clamp01(effect.chance);
    //        if (chance <= 0f) continue;
    //        if (chance < 1f && Random.value > chance) continue;
    //        if (effect.type == AbilityEffectType.ModifyStat)
    //        {
    //            string skillName = nameSkill;
    //            if (effect.target == AbilityTarget.CurrentTarget)
    //            {
    //                List<Transform> targets = heroControl.enemyTarget;
    //                for (int j = 0; j < targets.Count; j++)
    //                {
    //                    var targetUnit = targets[j].GetComponent<HeroControl>();
    //                    if (targetUnit == null) continue;
    //                    int duration = heroControl.ShouldPlus ? effect.durationTurn + 1 : effect.durationTurn;
    //                    targetUnit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, effect.durationTurn, effect.modifyValue, effect.stackCount, heroControl);

    //                }

    //                heroControl.HeroEventt.SetEffectToEnemy(TimesToCall.onAttack);


    //            }
    //        }
    //        else
    //        {
    //            foreach (var target in heroControl.enemyTarget)
    //            {

    //                HeroControl enemyControl = target.GetComponent<HeroControl>();
    //                if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
    //                Debug.Log(enemyControl.HeroInfo.ID);
    //                int damagePerTurn = (int)(heroControl.HeroStatRuntime.Damage * (effect.modifyValue / 100f));
    //                int duration = heroControl.IsStart ? effect.durationTurn : effect.durationTurn + 1;
    //                enemyControl.HeroStatRuntime.ApplyAES(nameSkill,effect.type, duration, damagePerTurn, effect.stackCount, heroControl);
    //            }
    //        }

    //    }
    //}
    void ApplyEffect(string nameSkill, List<AbilityEffect> listEffect)
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
                string skillName = nameSkill;
                if (effect.target == AbilityTarget.CurrentTarget)
                {
                    var targetUnit = currentTarget.GetComponent<HeroControl>();
                    if (targetUnit == null) continue;
                    int duration = heroControl.ShouldPlus ? effect.durationTurn + 1 : effect.durationTurn;
                    targetUnit.HeroStatRuntime.ApplyModifyStat(skillName, effect.statType, effect.durationTurn, effect.modifyValue, effect.stackCount, heroControl);



                }
                else if(effect.target == AbilityTarget.Self)
                {
                    int duration = heroControl.ShouldPlus ? effect.durationTurn + 1 : effect.durationTurn;
                    heroControl.HeroStatRuntime.ApplyModifyStat(nameSkill, effect.statType, effect.durationTurn, effect.modifyValue, effect.stackCount, heroControl);
                }
            }
            else
            {
                
                
                HeroControl enemyControl = currentTarget.GetComponent<HeroControl>();
                if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                int damagePerTurn = (int)(heroControl.HeroStatRuntime.Damage * (effect.modifyValue / 100f));
                int duration = heroControl.IsStart ? effect.durationTurn : effect.durationTurn + 1;
                enemyControl.HeroStatRuntime.ApplyAES(nameSkill, effect.type, duration, damagePerTurn, effect.stackCount, heroControl);
                
            }

        }
    }
}
