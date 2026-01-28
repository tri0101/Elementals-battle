using NUnit;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_ShowResource : MonoBehaviour, IObserver
{
    public static UI_ShowResource Instance;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI staminaText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }
    void Start()
    {
        PlayerInventory.Instance.AddObserver(this);
        PlayerInventory.Instance.NotifyCurrentResource();
    }
    public void OnNotify() { }
    public void OnNotify(object data)
    {
       
        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 1)
                coinText.text = value.ToString();
            else if (itemId == 2)
                diamondText.text = value.ToString();
        }
    }
}
