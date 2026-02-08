using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;

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
    }
 
    public void SetGainManaNormal()
    {
        heroControl.HeroStatRuntime.GainMana(heroControl.HeroInfo.normalAttack.manaGain);
    }
 
    public void SetGainManaSkill()
    {
        heroControl.HeroStatRuntime.GainMana(heroControl.HeroInfo.skill.manaGain);
    }
 


 
    
}
