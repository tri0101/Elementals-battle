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
    [SerializeField] int starHero;
    [Header("Star Colors")]
    public Color earnedColor = new Color32(255, 215, 0, 255);      // vàng
    public Color notEarnedColor = new Color32(158, 101, 101, 255); // tối
    void OnEnable()
    {
        LoadSkill();
       
    }
    
    public void LoadSkill()
    {
        Clear();
        CreateHeader();
        CreateSkill(AbilityType.Skill);
        CreateSkill(AbilityType.Ultimate);
        CreateSkill(AbilityType.Passive);
    }

    void CreateSkill(AbilityType type)
    {
        HeroViewData heroViewData = HeroUpgradeContext.SelectedHero;
        if (heroViewData == null) return;
        string nameSkill = "";
        Sprite icon = null;
        int cost = 0;
        int currentLevel = heroViewData.instance.GetAbilityLevel(type);
        switch (type)
        {
            case AbilityType.Skill:
                icon = heroViewData.info.skill.icon;
                nameSkill = heroViewData.info.skill.abilityName;
                cost = heroSkillCostConfig.costPerLevelSkill[currentLevel - 1];
                break;
            case AbilityType.Ultimate:
                icon = heroViewData.info.ultimate.icon;
                nameSkill = heroViewData.info.ultimate.abilityName;
                cost = heroSkillCostConfig.costPerLevelUltimate[currentLevel - 1];
                break;
            case AbilityType.Passive:
                icon = heroViewData.info.passive.icon;
                nameSkill = heroViewData.info.passive.abilityName;
                cost = heroSkillCostConfig.costPerLevelPassive[currentLevel - 1];
                break;
        }
        
        var go = Instantiate(UI_SkillItemPrefabs, content);
        var ui = go.GetComponent<UI_SkillUpgradeItem>();
        ui.Setup(icon,nameSkill, currentLevel, cost);
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
        foreach(Transform child in starRoot)
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
    
    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
