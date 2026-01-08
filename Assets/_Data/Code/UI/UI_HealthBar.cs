using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_HealthBar : MonoBehaviour, IObserver
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] Image fillImage;


    void Awake()
    {
        playerControl = transform.parent.parent.GetComponent<PlayerControl>();
        fillImage = transform.Find("FillBar").GetComponent<Image>();
    }
    private void Start()
    {
        playerControl.AddObserver(this);
        OnNotify();
    }

    private void OnDestroy()
    {
        playerControl.RemoveObbserver(this);
    }

    public void OnNotify()
    {
        fillImage.fillAmount = playerControl.PlayerReceiveDamagee.GetHealthPercent();
    }
}
