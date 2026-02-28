using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_PanelStoreDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameAndAmountText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private Image iconCurrency;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button buyButton;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        
    }

    public void SetUp(GameObject objItem, ItemData itemData, ShopItemData shopItemData, Sprite icon)
    {
        gameObject.SetActive(true);
        nameAndAmountText.text = $"{itemData.itemName} x{shopItemData.amount}";
        costText.text = shopItemData.price.ToString();
        iconCurrency.sprite = icon;
        ownedText.text = $"Owned: {GetOwnedAmount(itemData.id)}";
        
        GameObject obj = Instantiate(objItem, transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.anchoredPosition = new Vector2(-175, 225f);
        if(PlayerInventory.Instance.GetItemQuantity(shopItemData.currencyType == CurrencyType.Diamond ? 2 : 1) < shopItemData.price)
        {
            costText.color = Color.red;
            buyButton.interactable = false;
            
        }
        else
        {
            costText.color = Color.white;
            buyButton.interactable = true;
        }
            
        TextMeshProUGUI text = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "BUY";
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(()=> OnClickBuy(itemData.id, shopItemData.amount));
    }

    int GetOwnedAmount(int itemId)
    {
        return PlayerInventory.Instance.GetItemQuantity(itemId);
    }
    void OnClickBuy(int itemId ,int amount)
    {
        PlayerInventory.Instance.AddItem(itemId, amount);
        buyButton.interactable = false;
        TextMeshProUGUI text = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "SOLD";
    }
}
