using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LimitedOffer : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform contentPrefabNeed;
    [SerializeField] Transform contentPrefabExchange;
    [SerializeField] Button exchangeButton;
    [SerializeField] TextMeshProUGUI exchangeLimitText;
    BannerLimitedOfferData data;
    UI_GachaFeatured UI_GachaFeatured;
    bool isEnoughItem = true;
    bool isReachLimit => LimitedOfferState.Instance.GetRedeemedCount(data.id) >= data.exchangeLimit;
    public void SetUp(BannerLimitedOfferData data)
    {
        this.data = data;
        RefreshExchangeCount();
        ClearContentNeed();
        ClearContentExchange();
        SetUpPrefabNeed();
        SetUpPrefabExchange();
        RefreshExchangeButton();
        exchangeButton.onClick.RemoveAllListeners();
        exchangeButton.onClick.AddListener(() => OnClickExchange(data));

    }
    
    public void SetUIFeatured(UI_GachaFeatured ui)
    {
        UI_GachaFeatured = ui;
    }
    public void RefreshExchangeButton()
    {
        if(isReachLimit || !isEnoughItem)
        {
            exchangeButton.interactable = false;
        }
        else
        {
            exchangeButton.interactable = true;
        }
    }
    public void RefreshExchangeCount()
    {
        int count = LimitedOfferState.Instance.GetRedeemedCount(data.id);
        exchangeLimitText.text = $"{count}/{data.exchangeLimit}";
        if(isReachLimit)
        {
            exchangeLimitText.color = Color.red;
        }
        else
        {
            exchangeLimitText.color = Color.white;
        }
    }
    public void ClearContentExchange()
    {
        foreach(Transform child in contentPrefabExchange)
        {
            Destroy(child.gameObject);
        }
    }
    public void ClearContentNeed()
    {
        foreach (Transform child in contentPrefabNeed)
        {
            Destroy(child.gameObject);
        }
    }
    public void SetUpPrefabNeed()
    {
        List<ItemAndAmount> list = data.itemNeed;
        foreach (ItemAndAmount item in list)
        {
            GameObject go = Instantiate(itemPrefab,contentPrefabNeed);
            UI_DropRewardItem ui = go.GetComponent<UI_DropRewardItem>();
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(item.itemId);
            ui.Setup(itemData, item.amount);
            if (PlayerInventory.Instance.GetItemQuantity(item.itemId) < item.amount)
            {
                 ui.SetUpColorText(Color.red);
                 isEnoughItem = false;
            }
            else
            {
                 ui.SetUpColorText(Color.white);
                 
            }
        }
    }
    
    public void SetUpPrefabExchange()
    {
        List<ItemAndAmount> list = data.itemExchange;
        foreach (ItemAndAmount item in list)
        {
            GameObject go = Instantiate(itemPrefab,contentPrefabExchange);
            UI_DropRewardItem ui = go.GetComponent<UI_DropRewardItem>();
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(item.itemId);
            ui.Setup(itemData, item.amount);
        }
    }
    void OnClickExchange(BannerLimitedOfferData data)
    {
        foreach(ItemAndAmount itemAndAmount in data.itemNeed)
        {
            PlayerInventory.Instance.ConsumeItem(itemAndAmount.itemId, itemAndAmount.amount);
        }
        foreach (ItemAndAmount itemAndAmount in data.itemExchange)
        {
            
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemAndAmount.itemId);
            UI_CanvasReward.Instance.gameObject.SetActive(true);
            UI_CanvasReward.Instance.ClearOldItems();
            UI_CanvasReward.Instance.SetUp(itemData, itemAndAmount.amount);
        }

        UI_CanvasReward.Instance.ShowReward();
        LimitedOfferState.Instance.IncrementRedeem(data.id);    
        RefreshExchangeCount();
        UI_GachaFeatured.RefreshItemLimitedOffer();
        RefreshExchangeButton();

    }

    
}
