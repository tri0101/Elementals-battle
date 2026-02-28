using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_StoreItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI nameItemText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject shardItemPrefabs;
    [SerializeField] private GameObject itemPrefabs;
    [SerializeField] private Image iconCurrency;
    [SerializeField] private Sprite coin;
    [SerializeField] private Sprite diamond;
    [SerializeField] private Button buttonBuy;
    
    [Header("Transform")]
    [SerializeField] private Transform contentItem;


    public void SetUp(ItemData itemdata, ShopItemData shopItemData)
    {
        Sprite currencyTypeIcon = null;
        amountText.text = $"x{shopItemData.amount}";
        nameItemText.text = name;
        priceText.text = shopItemData.price.ToString();
        
        if (shopItemData.currencyType == CurrencyType.Gold)
        {
            if(PlayerInventory.Instance.GetItemQuantity(1) < shopItemData.price)
            {
                priceText.color = Color.red;
                buttonBuy.interactable = false;
            }
            else
            {
                priceText.color = Color.white;
                buttonBuy.interactable = true;
            }
            currencyTypeIcon = coin;
        } 
        else if (shopItemData.currencyType == CurrencyType.Diamond)
        {
            if (PlayerInventory.Instance.GetItemQuantity(2) < shopItemData.price)
            {
                priceText.color = Color.red;
                buttonBuy.interactable = false;
            }
            else
            {
                priceText.color = Color.white;
                buttonBuy.interactable = true;
            }
            currencyTypeIcon = diamond;
        }
        GameObject prefab =
            itemdata.type == ItemType.HeroShard
            ? (shardItemPrefabs ?? itemPrefabs)
            : itemPrefabs;
        iconCurrency.sprite = currencyTypeIcon;
        var go = Instantiate(prefab, contentItem);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(175f, 260f);
        var ui = go.GetComponent<UI_DropPreviewItem>();
        if (ui != null)
            ui.Setup(itemdata);
        buttonBuy.onClick.AddListener(() => OnClickBuy(go, itemdata, shopItemData, currencyTypeIcon));
    }
    public void OnClickBuy(GameObject objItem, ItemData itemData, ShopItemData shopItemData , Sprite currencySprite)
    {
        UI_PanelStoreDetail uI_PanelStoreDetail = transform.parent.parent.parent.Find("PanelDetail").GetComponent<UI_PanelStoreDetail>();
        uI_PanelStoreDetail.SetUp(objItem,itemData, shopItemData, currencySprite);

    }
}
