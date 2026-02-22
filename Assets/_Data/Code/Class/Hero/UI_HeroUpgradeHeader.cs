using System.Collections;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroUpgradeHeader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] int currentHeroId;
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private TextMeshProUGUI heroRoleText;
    [SerializeField] private TextMeshProUGUI heroPower;
    [SerializeField] private TextMeshProUGUI heroDamage;
    [SerializeField] private TextMeshProUGUI heroArmor;
    [SerializeField] private TextMeshProUGUI heroHealth;
    [SerializeField] private Transform heroPreviewPanel;

    [Header("Data")]
    [SerializeField] private HeroGrowthConfig growthConfig;
    public HeroGrowthConfig GrowthConfig => growthConfig;
    [SerializeField] private HeroLevelConfig levelConfig;

    [Header("Level Bar")]
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI currentLevelExpBarText;
    [SerializeField] private Image expFillImage;

    [Header("Upgrade FX")]
    [SerializeField] private float fxDuration = 0.1f;
    [SerializeField] private float fxScale = 1.5f;
    [SerializeField] private Color fxColor = Color.green;

    GameObject currentPreview;

    private Coroutine fxPowerRoutine;
    private Coroutine fxHealthRoutine;
    private Coroutine fxDamageRoutine;
    private Coroutine fxArmorRoutine;

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
            var stat = HeroStatCalculator.Calculate(data.info, data.instance, growthConfig);

            if (heroPower != null) heroPower.text = Mathf.RoundToInt(stat.power).ToString();
            if (heroHealth != null) heroHealth.text = Mathf.RoundToInt(stat.health).ToString();
            if (heroDamage != null) heroDamage.text = Mathf.RoundToInt(stat.damage).ToString();
            if (heroArmor != null) heroArmor.text = Mathf.RoundToInt(stat.armor).ToString();
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

    public void PlayStatUpgradeFx()
    {
        // Chỉ animate khi nâng cấp gọi vào (không auto khi Setup)
        PlayFx(heroPower, ref fxPowerRoutine);
        PlayFx(heroHealth, ref fxHealthRoutine);
        PlayFx(heroDamage, ref fxDamageRoutine);
        PlayFx(heroArmor, ref fxArmorRoutine);
    }

    private void PlayFx(TextMeshProUGUI txt, ref Coroutine routine)
    {
        if (txt == null) return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(AnimateTMP(txt, fxDuration, fxScale, fxColor));
    }

    private IEnumerator AnimateTMP(TextMeshProUGUI txt, float duration, float scaleUp, Color highlightColor)
    {
        if (txt == null) yield break;

        Transform t = txt.transform;
        Vector3 baseScale = t.localScale;
        Color baseColor = txt.color;

        float half = Mathf.Max(0.0001f, duration * 0.5f);

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / half);
            float eased = Mathf.SmoothStep(0f, 1f, k);

            t.localScale = Vector3.Lerp(baseScale, baseScale * scaleUp, eased);
            txt.color = Color.Lerp(baseColor, highlightColor, eased);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / half);
            float eased = Mathf.SmoothStep(0f, 1f, k);

            t.localScale = Vector3.Lerp(baseScale * scaleUp, baseScale, eased);
            txt.color = Color.Lerp(highlightColor, baseColor, eased);
            yield return null;
        }

        t.localScale = baseScale;
        txt.color = baseColor;
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