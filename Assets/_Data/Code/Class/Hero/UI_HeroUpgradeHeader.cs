using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroUpgradeHeader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] int currentHeroId;
    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroRoleText;
    public TextMeshProUGUI heroPower;
    public TextMeshProUGUI heroDamage;
    public TextMeshProUGUI heroArmor;
    public TextMeshProUGUI heroHealth;
    public Transform heroPreviewPanel;

    [Header("Data")]
    public HeroGrowthConfig growthConfig; 
    public HeroLevelConfig levelConfig; 
    [Header("Level Bar")]
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI currentLevelExpBarText;
    public Image expFillImage;

    GameObject currentPreview;

    public void Setup(HeroViewData data)
    {
        if (data == null) return;

        HeroRankHelper.GetRankVisual(
            data.instance.rank,
            data.info.Name,
            out string displayName,
            out Color color
        );

        if (data.info.ID != currentHeroId)
        {
            SetupPreview(data.info.HeroPreviewPrefabs);
        }
        currentHeroId = data.info.ID;

      
        heroNameText.text = displayName;
        heroNameText.color = color;
        heroRoleText.text = data.info.role.ToString();

        if (growthConfig != null)
        {
            var stat = HeroStatCalculator.Calculate(
                data.info,
                data.instance,
                growthConfig
            );

            if (heroPower != null)
                heroPower.text = Mathf.RoundToInt(stat.power).ToString();

            if (heroHealth != null)
                heroHealth.text = Mathf.RoundToInt(stat.health).ToString();

            if (heroDamage != null)
                heroDamage.text = Mathf.RoundToInt(stat.damage).ToString();

            if (heroArmor != null)
                heroArmor.text = Mathf.RoundToInt(stat.armor).ToString();
        }
        else
        {
           
            if (heroPower != null) heroPower.text = "-";
            if (heroHealth != null) heroHealth.text = "-";
            if (heroDamage != null) heroDamage.text = "-";
            if (heroArmor != null) heroArmor.text = "-";

           
        }
        float currentExp = data.instance.currentExp;
        float needExp = levelConfig.expPerLevel[data.instance.level - 1];
        currentLevelExpBarText.text = $"{currentExp} / {needExp}";
        currentLevelText.text = $"Lv. {data.instance.level}";
        expFillImage.fillAmount = needExp > 0 ? (float)currentExp / needExp : 0f;


    }

    void SetupPreview(GameObject previewPrefab)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        if (previewPrefab == null)
        {
            Debug.LogWarning("HeroPreviewPrefabs is NULL");
            return;
        }

        currentPreview = Instantiate(previewPrefab, heroPreviewPanel);
    }

  
}