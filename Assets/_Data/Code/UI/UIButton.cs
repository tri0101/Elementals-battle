using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour,IObserver
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] Image fillImage;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    string defaultName;
    private void Start()
    {
        playerControl.AddObserver(this);
        defaultName = textMeshProUGUI.text;
    }

    private void OnDestroy()
    {
        playerControl.RemoveObbserver(this);
    }
    public void OnNotify() { }
    public void OnNotify(object data)
    {
        if (data is PlayerControl playerControl)
        {
            float timeDurationSkill = playerControl.PlayerInfo.durationSkill;
            if (fillImage != null)
            {
                Color c = fillImage.color;
                c.a = 0.5f; 
                fillImage.color = c;
            }

            
            if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = timeDurationSkill.ToString("0.0");
                
            }
            StartCoroutine(ResetTime(timeDurationSkill));
        }

    }


    private IEnumerator ResetTime(float duration)
    {
        float timeLeft = duration;
        if (button != null)
            button.interactable = false;
        SetAlpha(0.5f);

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            if (textMeshProUGUI != null)
                textMeshProUGUI.text = timeLeft.ToString("0.0");

            yield return null;
        }


        if (textMeshProUGUI != null)
            textMeshProUGUI.text = "";
        if (button != null)
            button.interactable = true;
        textMeshProUGUI.text = defaultName;
        SetAlpha(1f);
        
    }

    private void SetAlpha(float alpha)
    {
        if (fillImage == null) return;

        Color c = fillImage.color;
        c.a = alpha;
        fillImage.color = c;
    }
}
