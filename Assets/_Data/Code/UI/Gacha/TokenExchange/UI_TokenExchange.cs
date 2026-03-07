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
        GameObject go = Instantiate(heroInfo.HeroPreviewPrefabs, contentPreview);
    }
}
