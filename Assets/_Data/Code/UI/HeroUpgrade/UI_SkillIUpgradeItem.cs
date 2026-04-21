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
    [SerializeField] private Button buttonDetail;
    [SerializeField] private string description; 

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
        string description = "",
        Action<HeroInstance, AbilityType> onClick = null,
        Func<AbilityType, int, int> getCostFunc = null)
    {
        boundHero = hero;
        boundAbilityType = abilityType;
        getCost = getCostFunc;
        this.description = description;
        if (icon != null) icon.sprite = Icon;
        if (nameText != null) nameText.text = name;

      
        ApplyLevelAndCost(currentLevel, cost);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(hero, abilityType));
        }

        if (buttonDetail != null)
        {
            buttonDetail.onClick.RemoveAllListeners();
            buttonDetail.onClick.AddListener(OnClickDetail);
        }
    }

    private void OnClickDetail()
    {
        var root = transform.parent != null ? transform.parent.parent : null;
        if (root == null) return;

        Transform panelRoot = root.Find("PanelDetailDes");
        Transform panel = panelRoot.GetChild(0);
        if (panel == null) return;

        var ui = panel.GetComponent<UI_SkillDetail>();
        if (ui == null) ui = panel.GetComponentInChildren<UI_SkillDetail>(true);
        if (ui == null) return;

        ui.transform.parent.gameObject.SetActive(true);

        string skillName = nameText != null ? nameText.text : string.Empty;
        string level = levelText != null ? levelText.text : string.Empty;
        string type = boundAbilityType.ToString();

        
        ui.SetUp(icon, skillName, level, type, this.description);
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

        CheckCoin();
    }
}