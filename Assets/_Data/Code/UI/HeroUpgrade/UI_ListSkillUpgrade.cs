using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ListSkillUpgrade : MonoBehaviour
{
    [SerializeField] private HeroSkillCostConfig heroSkillCostConfig;
    [SerializeField] private Transform content;
    [SerializeField] private Transform starRoot;

    [Header("Prefabs")]
    [SerializeField] private GameObject UI_SkillItemPrefabs;

    [Header("Data")]
    [SerializeField] private HeroGrowthConfig growthConfig;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textNameHero;
    [SerializeField] private TextMeshProUGUI textLevelHero;
    [SerializeField] private TextMeshProUGUI textRoleHero;
    [SerializeField] private TextMeshProUGUI textPowerHero;

    private HeroViewData heroViewData;

    [SerializeField] int starHero;

    [Header("Star Colors")]
    public Color earnedColor = new Color32(255, 215, 0, 255);      // vàng
    public Color notEarnedColor = new Color32(158, 101, 101, 255); // tối

    private readonly Dictionary<AbilityType, UI_SkillUpgradeItem> skillItemsByType = new();

    void OnEnable()
    {
        LoadSkill();
    }

    public void LoadSkill()
    {
        Clear();
        skillItemsByType.Clear();

        CreateHeader();
        CreateSkill(AbilityType.Skill);
        CreateSkill(AbilityType.Ultimate);
        CreateSkill(AbilityType.Passive);
    }

    void CreateSkill(AbilityType type)
    {
        heroViewData = HeroUpgradeContext.SelectedHero;
        if (heroViewData == null) return;

        string nameSkill = "";
        Sprite icon = null;

        int currentLevel = heroViewData.instance.GetAbilityLevel(type);
        int cost = RefreshCost(type, currentLevel);

        switch (type)
        {
            case AbilityType.Skill:
                icon = heroViewData.info.skill.icon;
                nameSkill = heroViewData.info.skill.abilityName;
                break;
            case AbilityType.Ultimate:
                icon = heroViewData.info.ultimate.icon;
                nameSkill = heroViewData.info.ultimate.abilityName;
                break;
            case AbilityType.Passive:
                icon = heroViewData.info.passive.icon;
                nameSkill = heroViewData.info.passive.abilityName;
                break;
        }

        var go = Instantiate(UI_SkillItemPrefabs, content);
        var ui = go.GetComponent<UI_SkillUpgradeItem>();

        ui.Setup(
            icon,
            nameSkill,
            currentLevel,
            cost,
            heroViewData.instance,
            type,
            OnClickUp,
            RefreshCost);

        skillItemsByType[type] = ui;
    }

    void CreateHeader()
    {
        HeroViewData heroViewData = HeroUpgradeContext.SelectedHero;
        textNameHero.text = heroViewData.info.Name;
        textLevelHero.text = heroViewData.instance.level.ToString();
        textRoleHero.text = heroViewData.info.role.ToString();

        int power = RefreshPower(heroViewData);
        textPowerHero.text = power.ToString();

        SetUpStar(heroViewData.instance.star);
    }

    void SetUpStar(int star)
    {
        foreach (Transform child in starRoot)
        {
            Image img = child.GetComponent<Image>();
            if (img == null) continue;

            if (child.GetSiblingIndex() < star)
                img.color = earnedColor;
            else
                img.color = notEarnedColor;
        }
    }

    int RefreshPower(HeroViewData data)
    {
        var stat = HeroStatCalculator.Calculate(
               data.info,
               data.instance,
               growthConfig
           );

        return starHero = Mathf.RoundToInt(stat.power);
    }

    int RefreshCost(AbilityType type, int currentLevel)
    {
        // prevent out of range when level grows beyond config
        int index = Mathf.Max(0, currentLevel - 1);

        switch (type)
        {
            case AbilityType.Skill:
                index = Mathf.Min(index, heroSkillCostConfig.costPerLevelSkill.Length - 1);
                return heroSkillCostConfig.costPerLevelSkill[index];

            case AbilityType.Ultimate:
                index = Mathf.Min(index, heroSkillCostConfig.costPerLevelUltimate.Length - 1);
                return heroSkillCostConfig.costPerLevelUltimate[index];

            case AbilityType.Passive:
                index = Mathf.Min(index, heroSkillCostConfig.costPerLevelPassive.Length - 1);
                return heroSkillCostConfig.costPerLevelPassive[index];
        }

        return 0;
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }

    void OnClickUp(HeroInstance hero, AbilityType type)
    {
        
        int cost = RefreshCost(type, hero.GetAbilityLevel(type));
        if (PlayerInventory.Instance.GetItemQuantity(1) < cost)
        {
            UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyCoin();
            return;
        }
        else
        {
            PlayerInventory.Instance.ConsumeItem(1, cost);
        }
        HeroUpgradeService.Instance.UpSkill(hero, type);
        // refresh power (header)
        textPowerHero.text = RefreshPower(heroViewData).ToString();

        // refresh that skill row: level + new cost + coin color
        if (skillItemsByType.TryGetValue(type, out var itemUi) && itemUi != null)
            itemUi.RefreshUI();
    }
}