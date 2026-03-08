using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_TokenExchange : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] Transform contentPreview;
    [Header("Text")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI redeemText;
    [SerializeField] TextMeshProUGUI amountCostText;
    [Header("Button")]
    [SerializeField] Button exchangeButton;


    public void SetUp(BannerTokenExchangeData data)
    {
        HeroInfo heroInfo = DatabaseManager.Instance.HeroDatabase.GetHero(data.heroId);
        nameText.text = heroInfo.Name;
        redeemText.text = $"Redeem: {data.redeemLimit}";
        amountCostText.text = $"{data.amountCost}";
        RefreshCost(data);
        RefreshRedeem(data);
        GameObject go = Instantiate(heroInfo.HeroPreviewPrefabs, contentPreview);
        exchangeButton.onClick.RemoveAllListeners();
        exchangeButton.onClick.AddListener(()=>ClickExchangeHero(data));  
    }
    void RefreshCost(BannerTokenExchangeData data)
    {
        if (PlayerInventory.Instance.GetItemQuantity(6) < data.amountCost)
        {
            amountCostText.color = Color.red;
            exchangeButton.interactable = false;
        }
        else
        {
            amountCostText.color = Color.white;
            exchangeButton.interactable = true;
        }
        
    }
    void RefreshRedeem(BannerTokenExchangeData data)
    {
        int redeemedCount = TokenExchangeState.Instance.GetRedeemedCount(data.heroId);
        redeemText.text = $"Redeem: {redeemedCount}/{data.redeemLimit}";
        if (redeemedCount >= data.redeemLimit)
        {
            exchangeButton.interactable = false;
        }   
    }

    void ClickExchangeHero(BannerTokenExchangeData data)
    {
        PlayerInventory.Instance.ConsumeItem(6, data.amountCost);
        UI_CanvasReward.Instance.gameObject.SetActive(true);
        UI_CanvasReward.Instance.ClearOldItems();
        UI_CanvasReward.Instance.SetUp(data);
        UI_CanvasReward.Instance.ShowReward();
        TokenExchangeState.Instance.IncrementRedeem(data.heroId);
        RefreshCost(data);
        RefreshRedeem(data);
    }
}
