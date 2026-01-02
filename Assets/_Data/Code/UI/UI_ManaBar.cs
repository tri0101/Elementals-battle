using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_ManaBar : MonoBehaviour, IObserver
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] Image fillImage;

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
        fillImage.fillAmount = playerControl.PlayerReceiveDamagee.GetManaPercent();
    }
}
