using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework.Interfaces;

public class UI_SummonReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject itemPrefab;


    public void SetUp(HeroTier tier)
    {
        tierText.text = $"{tier}";
        ClearContent();
        CreateHero(tier);
        
    }
    public void SetUpItem()
    {
        tierText.text = $"Other";
        foreach (ItemGachaData itemData in GachaManager.Instance.FeaturedBanner.itemPool)
        {
            GameObject itemGO = Instantiate(itemPrefab, content);
            UI_ItemSummonReward item = itemGO.GetComponent<UI_ItemSummonReward>();
            item.SetUp(itemData.itemId);
        }
    }
    void CreateHero(HeroTier tier)
    {
        foreach(HeroGachaData heroData  in GachaManager.Instance.FeaturedBanner.pool)
        {
            if(heroData.tier == tier)
            {
                GameObject heroGO = Instantiate(heroPrefab, content);
                UI_HeroSummonReward item = heroGO.GetComponent<UI_HeroSummonReward>();
                item.SetUp(heroData.heroId);
            }
            
        }
    }
    void ClearContent()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
