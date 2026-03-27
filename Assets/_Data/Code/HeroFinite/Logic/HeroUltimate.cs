using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class HeroUltimate : MonoBehaviour
{
    HeroControl heroControl;
    public HeroControl heroControlhero => heroControl;

    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }


    public void ResetMana()
    {
        heroControl.HeroStatRuntime.MinusMana(1000);
    }
    public void RefreshTotalDmg()
    {
        heroControl.HeroReceiveDamagee.RefreshTotalDmg();
    }
}
