using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class HeroDesertReceiveDamage : HeroReceiveDamagee
{
    [SerializeField] bool hasBeenFrenzy = false;
    public bool HasBeenFrenzy
    {
        get => hasBeenFrenzy;
        set
        {
            hasBeenFrenzy = value;
        }
    }
    bool hasCallFrenzy = false;

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
        return dmg;
    }

    public override void CallWaitForAttackerFinished(HeroControl attacker)
    {
        if (attacker == null)
            return;
        heroControl.HeroEventt.RefreshHasShown();
        StartCoroutine(WaitForAttackerFinished(attacker));
    }

    IEnumerator WaitForAttackerFinished(HeroControl attacker)
    {
        yield return new WaitUntil(() => attacker.IsFinished);
        if (attacker == null)
            yield break;
        if (heroControl.HeroStatRuntime.CurrentHealth <= heroControl.HeroStatRuntime.MaxHealth * 0.5f && !heroControl.IsDead && !hasBeenFrenzy)
        {
            StartCoroutine(CoFrenzyFlow(attacker));
        }

        if (heroControl.HeroInfo.ID == 512 && hasBeenFrenzy && attacker.CurrentStringState == HeroStateManager.hero_Ultimate)
        {
            Debug.Log("Desert is waiting for attack to finish before counterattacking.");
            StartCoroutine(CoCounterAttackFlow(attacker));
        }
        heroControl.HeroEventt.RefreshHasShown();
        heroControl.HeroEventt.RefreshFirstAttack();
    }

    public override void CallTakeHit(bool shouldTakeHit)
    {
        if (shouldTakeHit)
        {
            heroControl.SetIsTakeHit();
        }
    }

    private IEnumerator CoFrenzyFlow(HeroControl attacker)
    {
        heroControl.IsFinished = false;

        yield return new WaitUntil(() => attacker.IsFinished);

        hasBeenFrenzy = true;
        SetInsane();
        heroControl.CanSkill = true;
        if (!hasCallFrenzy)
        {
            HeroControl.RefreshObservers("Frenzy Activated");
            hasCallFrenzy = true;
        }
            
        if (heroControl.HeroInfo.ID == 512)
        {
            heroControl.ChangeAnimationAnyState("Frenzy");
            yield break;
        }
        heroControl.IsFinished = true;
        
    }

    private IEnumerator CoCounterAttackFlow(HeroControl attacker)
    {
        heroControl.IsFinished = false;

        yield return new WaitUntil(() => attacker.IsFinished);
        yield return new WaitForSeconds(1f);

        heroControl.SetTarget(new List<Transform> { attacker.transform }, heroControl.HeroInfo.normalAttack);
        heroControl.SetAttack();

        yield return new WaitUntil(() => heroControl.IsFinished);
    }

    void SetInsane()
    {
        heroControl.IsCrit = true;
        heroControl.HeroStatRuntime.skillChanceFinal = 1f;
        heroControl.HeroStatRuntime.CritRate = 50f;
        heroControl.HeroStatRuntime.CritDamage = 150f;
        heroControl.HeroStatRuntime.Damage *= 1.5f;
        heroControl.HeroStatRuntime.Armor *= 2f;
    }
}