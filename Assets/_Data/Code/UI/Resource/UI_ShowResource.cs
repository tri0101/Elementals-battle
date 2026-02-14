using NUnit;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_ShowResource : MonoBehaviour, IObserver
{
    public static UI_ShowResource Instance;
    [SerializeField] private UI_Exchange uI_Exchange;
    public UI_Exchange UI_Exchange => uI_Exchange;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private Button buttonBuyStamina;
    [SerializeField] private Button buttonBuyCoin;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        uI_Exchange = GetComponent<UI_Exchange>();
        buttonBuyStamina.onClick.AddListener(UI_Exchange.ShowPanelBuyStamina);
        buttonBuyCoin.onClick.AddListener(UI_Exchange.ShowPanelBuyCoin);
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
            else if (itemId == 3)
                staminaText.text = value.ToString();
        }
    }
}
