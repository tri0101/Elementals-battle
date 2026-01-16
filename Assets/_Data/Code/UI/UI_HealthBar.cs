using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_HealthBar : MonoBehaviour, IObserver
{
    [SerializeField] HeroControl heroControl;
    [SerializeField] Image fillImage;


    void Awake()
    {
        heroControl = transform.parent.parent.GetComponent<HeroControl>();
        fillImage = transform.Find("FillBar").GetComponent<Image>();
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
        fillImage.fillAmount = heroControl.HeroReceiveDamagee.GetHealthPercent();
    }
}
