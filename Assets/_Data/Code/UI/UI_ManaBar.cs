using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_ManaBar : MonoBehaviour, IObserver
{
    [SerializeField] HeroControl heroControl;
    [SerializeField] Image fillSImage;
    [SerializeField] Image fillTImage;
    const float FIRST_BAR_MAX = 500f;
 

    void Awake()
    {
        heroControl = transform.parent.parent.GetComponent<HeroControl>();
        fillSImage = transform.Find("FillSBar").GetComponent<Image>();
        fillTImage = transform.Find("FillTBar").GetComponent<Image>();
    }
    private void Start()
    {
        heroControl.AddObserver(this);
        OnNotify();
    }

    private void OnDestroy()
    {
        heroControl.RemoveObbserver(this);
    }

    public void OnNotify()
    {
        float mana = heroControl.HeroReceiveDamagee.Mana;
        //fillSImage.fillAmount = heroControl.heroReceiveDamagee.GetManaPercent();
        float manaS = Mathf.Clamp(mana, 0, FIRST_BAR_MAX);
        fillSImage.fillAmount = manaS / FIRST_BAR_MAX;

        // Thanh T (250 → 500)
        float manaT = Mathf.Clamp(mana - FIRST_BAR_MAX, 0, FIRST_BAR_MAX);
        fillTImage.fillAmount = manaT / FIRST_BAR_MAX;
    }
}
