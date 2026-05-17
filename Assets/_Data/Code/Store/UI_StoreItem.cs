using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UI_StoreItem : MonoBehaviour, IObserver
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
    [SerializeField] private Image overlay;
    [SerializeField] private float glowSpeed = 6f;
    private bool isSold = false;
    [Header("Transform")]
    [SerializeField] private Transform contentItem;

    private CurrencyType currencyType;
    private ShopItemData shopItemData;

    void OnDisable()
    {
        PlayerInventory.Instance.RemoveObbserver(this);
        buttonBuy.onClick.RemoveAllListeners();
    }

    public void SetUp(ItemData itemdata, ShopItemData shopItemData)
    {
        PlayerInventory.Instance.AddObserver(this);

        this.shopItemData = shopItemData;
        currencyType = shopItemData.currencyType;

        amountText.text = $"x{shopItemData.amount}";
        nameItemText.text = itemdata.itemName;
        priceText.text = shopItemData.price.ToString();

        iconCurrency.sprite = currencyType == CurrencyType.Gold ? coin : diamond;

        RefreshCostText();

        GameObject prefab =
            itemdata.type == ItemType.HeroShard
            ? (shardItemPrefabs ?? itemPrefabs)
            : itemPrefabs;

        var go = Instantiate(prefab, contentItem);
        int overlayIndex = overlay.transform.GetSiblingIndex();
        go.transform.SetSiblingIndex(overlayIndex);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(175f, 260f);

        var ui = go.GetComponent<UI_DropPreviewItem>();
        if (ui != null)
            ui.Setup(itemdata);

        if(isSold)
        {
            SetSoldOut();
            return;
        }
        buttonBuy.onClick.RemoveAllListeners();
        buttonBuy.onClick.AddListener(() =>
        {
            UI_PanelStoreDetail panel =
                transform.root.Find("PanelDetail").GetComponent<UI_PanelStoreDetail>();

            panel.SetUp(go, itemdata, shopItemData, iconCurrency.sprite, this);
         
        });
    }
    public void SetSoldOut()
    {
        isSold = true;
        buttonBuy.interactable = false;
        buttonBuy.GetComponentInChildren<TextMeshProUGUI>().text = "SOLD";
        

    }
    void RefreshCostText()
    {
        int currencyId = currencyType == CurrencyType.Diamond ? 2 : 1;

        if (PlayerInventory.Instance.GetItemQuantity(currencyId) < shopItemData.price)
            priceText.color = Color.red;
        else
            priceText.color = Color.white;
    }

    public void OnNotify(object data)
    {
        if (data is (int itemId, int value))
        {
            if (itemId == 1 || itemId == 2)
                RefreshCostText();
        }
    }
    public IEnumerator PlayGlow(float duration)
    {
        if (overlay == null) yield break;

        float timer = 0f;
        Color baseColor = overlay.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = 0.5f + Mathf.Sin(Time.time * glowSpeed) * 0.5f;

            overlay.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            yield return null;
        }

        overlay.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
    }
}