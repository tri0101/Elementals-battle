using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEventt : MonoBehaviour
{
  

    HeroControl heroControl;

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
                    ShowUIEffect(effect.statType);
                else if(effect.target == AbilityTarget.CurrentTarget)
                {
                    foreach (var target in heroControl.enemyTarget)
                    {
                        HeroControl enemyControl = target.GetComponent<HeroControl>();
                        if (enemyControl == null || enemyControl.HeroStatRuntime == null) continue;
                        ShowUIEffect(effect.statType);
                    }
                }
            }
        }
    }
    void ShowUIEffect(ModifyStatType type)
    {
        switch(type)
        {
            case ModifyStatType.CritRate:
                heroControl.RefreshObservers(ModifyStatType.CritRate);
                break;
            case ModifyStatType.Armor:
                heroControl.RefreshObservers(ModifyStatType.Armor);
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

 
    
}
