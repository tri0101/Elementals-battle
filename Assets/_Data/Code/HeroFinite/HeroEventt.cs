using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroEventt : MonoBehaviour
{

    public LoadNormalAttack load;
    HeroControl heroControl;
    private Coroutine hideTotalDmgRoutine;
    private void Awake()
    {
        heroControl = transform.parent.GetComponent<HeroControl>();
    }

    public void SetFinished()
    {
        heroControl.IsFinished = true;
        heroControl.NotifyActionFinished();
    }
 
    public void SetGainManaNormal()
    {
        heroControl.HeroStatRuntime.GainMana(heroControl.HeroInfo.normalAttack.manaGain);
    }
 
    public void SetGainManaSkill()
    {
        heroControl.HeroStatRuntime.GainMana(heroControl.HeroInfo.skill.manaGain);
    }
    public void SetEffect()
    {
        List<AbilityEffect> effectOnAttack = heroControl.HeroInfo.ultimate.GetEffectsOnAttack();
        for (int i = 0; i < effectOnAttack.Count; i++)
        {
            var effect = effectOnAttack[i];
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                if (effect.target == AbilityTarget.Self)
                    ShowTextEffect(effect.statType);
                else if(effect.target == AbilityTarget.CurrentTarget)
                {
                    foreach (var target in heroControl.enemyTarget)
                    {
                        HeroControl enemyControl = target.GetComponent<HeroControl>();
                        if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                        enemyControl.HeroEventt.ShowTextEffect(effect.statType);
                    }
                }
            }
        }
    }
    public void ShowTextEffect(ModifyStatType type)
    {
        switch(type)
        {
            case ModifyStatType.CritRate:
                heroControl.RefreshObservers(ModifyStatType.CritRate);
                break;
            case ModifyStatType.ArmorDecreased:
                heroControl.RefreshObservers(ModifyStatType.ArmorDecreased);
                break;
            case ModifyStatType.ArmorIncreased:
                heroControl.RefreshObservers(ModifyStatType.ArmorIncreased);
                break;
        }
       
    }
    public void ShowUIEffect(AbilityEffectType type)
    {
        switch(type)
        {
            case AbilityEffectType.Rooted:
                foreach (var target in heroControl.enemyTarget)
                {
                    HeroControl enemyControl = target.GetComponent<HeroControl>();
                    if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                    enemyControl.HeroStatRuntime.ApplyEartEffect();
                }
                break;
        }
    }
    public void NotifyCanDead()
    {
        foreach (Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;
            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;
            heroC.HeroReceiveDamagee.SetCanDead();
        }
    }
    public void NotifyCanShowTotalDmg()
    {
        foreach (Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;
            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;
            heroC.HeroReceiveDamagee.SetCanShowTotalDmg();
        }
    }
    public void NotifyCanHideTotalDmg()
    {
        if (hideTotalDmgRoutine != null)
            StopCoroutine(hideTotalDmgRoutine);

        hideTotalDmgRoutine = StartCoroutine(CoHideTotalDmgAfterDelay(0.5f));
    }

    private IEnumerator CoHideTotalDmgAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform enemy in heroControl.enemyTarget)
        {
            if (enemy == null) continue;

            HeroControl heroC = enemy.GetComponent<HeroControl>();
            if (heroC == null) continue;

            var canvas = heroC.CanvasTotalDamage;
            if (canvas == null) continue;

            canvas.Hide();
        }

        hideTotalDmgRoutine = null;
    }
    public void SpawnObject(int index)
    {
        TypeAndVector typeAndvector = load.dicSpawn.Find(dic => dic.indexSpawn == index);
        if(typeAndvector == null) return;
        Vector3 newPos = heroControl.transform.position;
        newPos.x += typeAndvector.positionSpawn.x;
        newPos.y += typeAndvector.positionSpawn.y;
        List<Transform> listEnemy = heroControl.enemyTarget;
        EffectManager.Instance.Spawn(typeAndvector.type, EffectManager.Instance.ListEffect, listEnemy, newPos);
    }

    public void CallStopAnim()
    {
        if(!heroControl.CanAttackInBattle)
        heroControl.Animator.speed = 0f;
    }
    public void CallCancelStopAnim()
    {
        
        heroControl.Animator.speed = 1f;    
    }
    
}
