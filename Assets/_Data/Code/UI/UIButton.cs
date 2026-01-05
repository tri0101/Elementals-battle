using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class UIButton : MonoBehaviour,IObserver
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] Image fillImage;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    string defaultName;
    [SerializeField] string buttonName;
    float duration;
    private void Start()
    {
        playerControl.AddObserver(this);
        defaultName = textMeshProUGUI.text;
        buttonName = transform.name;
        button = transform.GetChild(0).GetChild(0).GetComponent<Button>();
        if(buttonName == "Transform")
        {
            button.interactable = false;
        }
      
    }

    private void OnDestroy()
    {
        playerControl.RemoveObbserver(this);
    }

    public void OnNotify()
    {
        if(playerControl.PlayerReceiveDamagee.Mana >= 1000f && transform.name == "Transform")
        {
            button.interactable = true;
        }
    }
    public void OnNotify(object data)
    {
        if (data is ValueTuple<string, float> tuple)
        {
            string stringObserver = tuple.Item1;
            float duration = tuple.Item2;
            
            
            Debug.Log( duration);
            if (stringObserver == buttonName)
            {
                
                
                StartCoroutine(ResetTime(duration));
            }
        }
        else if (data is string value)
        {
            if (value == buttonName)
            {
                button.interactable = false;
            }
        }
    }



    private IEnumerator ResetTime(float duration)
    {
        float timeLeft = duration;
        if (button != null)
        {
           
            button.interactable = false;
        }
            
        
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
        

    }

    private void SetAlpha(float alpha)
    {
        if (fillImage == null) return;

        Color c = fillImage.color;
        c.a = alpha;
        fillImage.color = c;
    }
}
