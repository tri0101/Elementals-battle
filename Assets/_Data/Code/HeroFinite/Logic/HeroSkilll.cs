using UnityEngine;

public class HeroSkilll : MonoBehaviour
{
    HeroControl heroControl;
    public HeroControl heroControlhero => heroControl;
    
    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }


    public void ResetMana()
    {
        heroControl.HeroReceiveDamagee.Mana -= 500f ;
        heroControl.RefreshObservers();
    }
}
