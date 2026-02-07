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
 
    public void SetGainMana()
    {
        heroControl.HeroStatRuntime.GainMana(100);
    }
 


 
    
}
