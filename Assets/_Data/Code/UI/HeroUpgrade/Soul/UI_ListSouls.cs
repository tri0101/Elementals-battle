using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UI_ListSouls : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject soulItemPrefab;
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
        if (heroViewData == null) return;
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

    // NEW: clear panel state before re-populating
    private void ClearUpgradePanel()
    {
        if (description != null) description.text = string.Empty;

        if (costUp != null)
        {
            costUp.text = string.Empty;
            costUp.color = Color.white;
        }

        if (buttonUp != null)
            buttonUp.interactable = false;
    }

    public void RefreshPanelUpgrade(SoulViewData soulData, int index)
    {
        ClearUpgradePanel();

        string des = "";
        float value = 0;
        float levelValue = soulData.instance.level;

        currentIndexShown = index;

        // if max: only show "Max" + description + cost infinity
        if (soulData.instance.level >= 10)
        {
            if (currentLevel != null) currentLevel.text = "Max";

            des = soulData.info != null ? soulData.info.description : string.Empty;
            if (description != null) description.text = des;

            if (costUp != null)
            {
                int current = PlayerInventory.Instance != null ? PlayerInventory.Instance.GetItemQuantity(5) : 0;
                costUp.text = $"-";
                costUp.color = Color.white;
            }

            if (buttonUp != null)
                buttonUp.interactable = false;

            return;
        }

        if (currentLevel != null) currentLevel.text = "Lv." + levelValue.ToString();

        HeroInstance heroInstance = heroViewData.instance;
        int currentNormal = PlayerInventory.Instance.GetItemQuantity(5);
        int level = soulData.instance.level;
        int required = 0;

        switch (index)
        {
            case 0:
                required = soulCostConfig.costSoul1[level - 1];
                value = soulData.info.soulValueConfigs[heroInstance.GetLevelSoul(0) - 1].value;
                if (soulData.info.soulName == "Guardian Soul") des += "Restore " + value.ToString() + " mana when hit";
                else if (soulData.info.soulName == "Arcane Soul") des += "Restore " + value.ToString() + " mana upon attacking";
                else if (soulData.info.soulName == "Warhammer Soul") des += "Gain " + value.ToString() + " mana at the start of battle";
                break;

            case 1:
                required = soulCostConfig.costSoul2[level - 1];
                value = soulData.info.soulValueConfigs[heroInstance.GetLevelSoul(1) - 1].value;
                des += "Increase skill rate by " + value.ToString() + "%";
                break;

            case 2:
                required = soulCostConfig.costSoul3[level - 1];
                value = soulData.info.soulValueConfigs[heroInstance.GetLevelSoul(2) - 1].value;
                des += "Increase control-free by " + value.ToString() + "%";
                break;
        }

        if (description != null) description.text = des;

        if (costUp != null) costUp.text = $"{currentNormal}/{required}";

        if (currentNormal < required)
        {
            if (costUp != null) costUp.color = Color.red;
            if (buttonUp != null) buttonUp.interactable = false;
        }
        else
        {
            if (costUp != null) costUp.color = Color.white;
            if (buttonUp != null) buttonUp.interactable = true;
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