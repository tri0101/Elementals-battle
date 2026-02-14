using TMPro;
using UnityEngine;

public class UI_ListSkillUpgrade : MonoBehaviour
{
    [SerializeField] private HeroSkillCostConfig heroSkillCostConfig;
    [SerializeField] private Transform content;

    [Header("Prefabs")]
    [SerializeField] private GameObject UI_SkillItemPrefabs;
 


    void OnEnable()
    {
        LoadSkill();
       
    }

    void LoadSkill()
    {
        Clear();
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
    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
