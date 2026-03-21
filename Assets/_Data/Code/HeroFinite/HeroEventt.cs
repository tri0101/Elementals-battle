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
        }
       
    }

 
    
}
