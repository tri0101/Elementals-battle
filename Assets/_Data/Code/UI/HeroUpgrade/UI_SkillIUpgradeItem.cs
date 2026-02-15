using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_SkillUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costUp;
    [SerializeField] private Button button;

    private Func<AbilityType, int, int> getCost;
    private HeroInstance boundHero;
    private AbilityType boundAbilityType;

    private int currentCost;

    private void Update()
    {
        CheckCoin();
        
    }
   
    void CheckCoin()
    {
        if (costUp == null) return;
        if (PlayerInventory.Instance == null) return;

        if (PlayerInventory.Instance.GetItemQuantity(1) >= currentCost)
            costUp.color = Color.white;
        else
            costUp.color = Color.red;
    }

    public void Setup(
        Sprite Icon,
        string name,
        int currentLevel,
        int cost,
        HeroInstance hero,
        AbilityType abilityType,
        Action<HeroInstance, AbilityType> onClick = null,
        Func<AbilityType, int, int> getCostFunc = null)
    {
        boundHero = hero;
        boundAbilityType = abilityType;
        getCost = getCostFunc;

        if (icon != null) icon.sprite = Icon;
        if (nameText != null) nameText.text = name;

        // initial ui
        ApplyLevelAndCost(currentLevel, cost);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(hero, abilityType));
        }
    }

    public void RefreshUI()
    {
        if (boundHero == null) return;

        int level = boundHero.GetAbilityLevel(boundAbilityType);

        int cost = currentCost;
        if (getCost != null)
            cost = getCost(boundAbilityType, level);

        ApplyLevelAndCost(level, cost);
    }

    private void ApplyLevelAndCost(int level, int cost)
    {
        currentCost = cost;

        if (levelText != null)
            levelText.text = "Lv." + level.ToString();

        if (costUp != null)
            costUp.text = cost.ToString();

        CheckCoin(); // refresh color immediately after changes
    }
}