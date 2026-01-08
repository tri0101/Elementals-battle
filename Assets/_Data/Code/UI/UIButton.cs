using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    private EventTrigger eventTrigger;


    void Awake()
    {
        playerControl = transform.parent.parent.GetComponent<PlayerControl>();
        fillImage = transform.GetChild(0).GetComponent<Image>();
        button = fillImage.transform.GetChild(0).GetComponent<Button>();
        textMeshProUGUI = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

    }
    private void Start()
    {
        playerControl.AddObserver(this);
        defaultName = textMeshProUGUI.text;
        buttonName = transform.name;
        Addlistener();
        if (buttonName == "Transform")
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
    void Addlistener()
    {
        if(buttonName == "Transform")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnTransformButtonDown);
        }
        else if (buttonName == "Jump")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnJumpButtonDown);
        }
        else if (buttonName == "NormalAttack")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnAttackButtonDown);
        }
        else if (buttonName == "Roll")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnRollButtonDown);
        }
        else if (buttonName == "RangedAttack")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnRangedAttackButtonDown);
        }
        else if (buttonName == "Skill_1")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnSKillOneButtonDown);
        }
        else if (buttonName == "Skill")
        {
            button.onClick.AddListener(playerControl.PlayerInput.OnSKillButtonDown);
        }
        else 
        {
            eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            AddEvent(EventTriggerType.PointerDown, OnPointerDown);
            AddEvent(EventTriggerType.PointerUp, OnPointerUp);
        }
       
    }
    void AddEvent(EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        eventTrigger.triggers.Add(entry);
    }
    void OnPointerDown(BaseEventData data)
    {
        if(buttonName == "MoveLeft")
        {
            playerControl.PlayerInput.OnMoveLeftDown();
        }
        else if(buttonName == "MoveRight")
        {
            playerControl.PlayerInput.OnMoveRightDown();
        }
        else if(buttonName == "Block")
        {
            playerControl.PlayerInput.OnBlockDown();
        }
        
    }
    void OnPointerUp(BaseEventData data)
    {
        if (buttonName == "MoveLeft")
        {
            playerControl.PlayerInput.OnMoveLeftUp();
        }
        else if (buttonName == "MoveRight")
        {
            playerControl.PlayerInput.OnMoveRightUp();
        }
        else if (buttonName == "Block")
        {
            playerControl.PlayerInput.OnBlockUp();
        }
    }
    private void SetAlpha(float alpha)
    {
        if (fillImage == null) return;

        Color c = fillImage.color;
        c.a = alpha;
        fillImage.color = c;
    }
}
