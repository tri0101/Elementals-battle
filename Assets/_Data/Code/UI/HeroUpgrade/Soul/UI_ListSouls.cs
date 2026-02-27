using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UI_ListSouls : MonoBehaviour
{
    [SerializeField ] private Transform content;
    [SerializeField ] private GameObject soulItemPrefab;
    HeroViewData heroViewData;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button buttonUp;
    [SerializeField] private TextMeshProUGUI costUp;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private SoulCostConfig soulCostConfig;
    int currentIndexShown;
    public void OnEnable()
    {
        
        LoadSouls();
    }
    private void Start()
    {
        buttonUp.onClick.RemoveAllListeners();
        buttonUp.onClick.AddListener(UpgradeSoul);
    }

    public void LoadSouls()
    {
        heroViewData = HeroUpgradeContext.SelectedHero;
        Clear();
        int i = 0;
        foreach (var soulInstance in heroViewData.instance.soulsInstances)
        {
            SoulViewData soulData = GetSoulViewData(soulInstance);

            var soulItemGO = Instantiate(soulItemPrefab, content);
            var soulItem = soulItemGO.GetComponent<UI_SoulItem>();

            ApplyPosition(soulItemGO.GetComponent<RectTransform>(), i);

            soulItem.SetUp(soulData, i, RefreshPanelUpgrade);

            i++;
        }
        RefreshPanelUpgrade(GetSoulViewData(heroViewData.instance.soulsInstances[0]), 0);
    }
    
    SoulViewData GetSoulViewData(FightSoulInstance soulInstance)
    {
        SoulViewData soulData = new SoulViewData
        {
            info = DatabaseManager.Instance.FightSoulDatabase.GetSoulInfo(soulInstance.soulID),
            instance = soulInstance
        };
        return soulData;
    }
    public void UpgradeSoul()
    {
        if(heroViewData == null) return;
        MinusItems();
        HeroUpgradeService.Instance.UpSoul(heroViewData.instance, currentIndexShown);
       
        RefreshPanelUpgrade(GetSoulViewData(heroViewData.instance.soulsInstances[currentIndexShown]), currentIndexShown);

    }
    void MinusItems()
    {
        int level = heroViewData.instance.soulsInstances[currentIndexShown].level;
        int required = 0;
        switch (currentIndexShown)
        {
            case 0:
                required = soulCostConfig.costSoul1[level - 1];
                break;
            case 1:
                required = soulCostConfig.costSoul2[level - 1];
                break;
            case 2:
                required = soulCostConfig.costSoul3[level - 1];
                break;
        }
        PlayerInventory.Instance.ConsumeItem(5, required);
    }
    public void RefreshPanelUpgrade(SoulViewData soulData , int index)
    {
        description.text = soulData.info.description;
        currentLevel.text = $"Lv. {soulData.instance.level}";
        int current = PlayerInventory.Instance.GetItemQuantity(5);
        int level = soulData.instance.level;
        int required = 0;
        currentIndexShown = index;
        switch (index)
        {
            case 0:
                required = soulCostConfig.costSoul1[level - 1];
                break;
            case 1:
                required = soulCostConfig.costSoul2[level - 1];
                break;
            case 2:
                required = soulCostConfig.costSoul3[level - 1];
                break;
        }
        costUp.text = $"{current}/{required}";
        if (current < required)
        {
            costUp.color = Color.red;
            buttonUp.interactable = false;

        }
        else
        {
            costUp.color = Color.white;
            buttonUp.interactable = true;
        }
        
    }
    
    public void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    private void ApplyPosition(RectTransform rect, int index)
    {
        switch (index)
        {
            case 0: // Soul 1
                rect.anchoredPosition = new Vector2(-440f, 100f);
                break;

            case 1: // Soul 2
                rect.anchoredPosition = new Vector2(-180f, -50f);
                break;

            case 2: // Soul 3
                rect.anchoredPosition = new Vector2(120f, 60f);
                break;
        }
    }
}
