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
        int manaGain = heroControl.HeroInfo.normalAttack.manaGain;
        foreach (var soul in heroControl.HeroInfo.soulID)
        {
            if (soul == 4)
            {
                FightSoulInfo soulInfo = DatabaseManager.Instance.FightSoulDatabase.GetSoulInfo(soul);
                if (soulInfo != null)
                {
                    HeroInstance heroInstance = PlayerInventory.Instance.GetHeroInstance(heroControl.HeroInfo.ID);
                    int manaAdd = soulInfo.soulValueConfigs[heroInstance.GetLevelSoul(0) - 1].value;
                    manaGain += manaAdd;
                }
            }
           
        }
        heroControl.HeroStatRuntime.GainMana(manaGain);
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
                    ShowTextEffect(effect.statType,(int)effect.modifyValue);
               
            }
        }
    }
    public void SetEffectToEnemy(TimesToCall timeTocall)
    {
        List<AbilityEffect> effects = null;
        if (timeTocall == TimesToCall.onAttack)
        {
            effects = heroControl.HeroInfo.ultimate.GetEffectsOnAttack();
        }
        else if(timeTocall == TimesToCall.OnUse)
        {
            effects = heroControl.HeroInfo.ultimate.GetEffectsOnUse();

        }
            
        for (int i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            if (effect.type == AbilityEffectType.ModifyStat)
            {
                if (effect.target == AbilityTarget.CurrentTarget)
                {
                    foreach (var target in heroControl.enemyTarget)
                    {
                        HeroControl enemyControl = target.GetComponent<HeroControl>();
                        if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                        enemyControl.HeroEventt.ShowTextEffect(effect.statType, (int)effect.modifyValue);
                    }
                }
            }
        }
    }
    public void ShowTextEffect(ModifyStatType type, int value)
    {
        switch(type)
        {
            case ModifyStatType.CritRate:
                heroControl.RefreshObservers(ModifyStatType.CritRate, value);
                break;
            case ModifyStatType.Armor:
                heroControl.RefreshObservers(ModifyStatType.Armor, value);
                break;
            case ModifyStatType.HealingRate:
                heroControl.RefreshObservers(ModifyStatType.HealingRate, value);
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
    


    public void GainHPAllHero() // chỉ dành cho hero có id = 8
    {
        AbilityInfo abilityInfo = heroControl.HeroInfo.skill;
        AbilityEffect abilityeffect = abilityInfo.effects[0];

        float damage = heroControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
            ModifyStatType.Damage,
            heroControl.HeroStatRuntime.Damage
        );

        // modifyValue = 20 nghĩa là 20%
        int hpGain = Mathf.RoundToInt(damage * (abilityeffect.modifyValue / 100f));

        Debug.Log(hpGain);
        foreach (Transform child in heroControl.transform.parent)
        {
            HeroControl heroC = child.GetComponent<HeroControl>();
            if (heroC == null) continue;

            heroC.HeroStatRuntime.GainHP(hpGain, DamageType.normalDamage);
            
        }
    }
}
