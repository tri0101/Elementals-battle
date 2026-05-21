using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;

public class UI_PanelHeroRestaurant : MonoBehaviour
{
    [SerializeField] private HeroGrowthConfig growthConfig;

    [Header("Content")]
    [SerializeField] private Transform contentListHero;
    [SerializeField] private Transform contentPreview;
    [SerializeField] private Transform contentSpeedFood;

    [Header("Prefabs")]
    [SerializeField] private GameObject heroItemPrefab;
    [SerializeField] private GameObject speedFoodPrefab;

    [Header("Buttons")]
    [SerializeField] private Button buttonAll;
    [SerializeField] private Button buttonDPS;
    [SerializeField] private Button buttonTank;
    [SerializeField] private Button buttonSupport;

    [Header("Button Sprites")]
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI nameHeroText;
    [SerializeField] private TextMeshProUGUI speed;

    [Header("Level Bar")]
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI currentLevelExpBarText;
    [SerializeField] private Image expFillImage;

    private Image imageAll;
    private Image imageDPS;
    private Image imageTank;
    private Image imageSupport;

    Coroutine expAnimRoutine;
    private Coroutine levelTextAnimRoutine;

    private RoleHero? currentFilter = null;
    private HeroViewData currentPreviewHero;

    void Awake()
    {
        imageAll = buttonAll.GetComponent<Image>();
        imageDPS = buttonDPS.GetComponent<Image>();
        imageTank = buttonTank.GetComponent<Image>();
        imageSupport = buttonSupport.GetComponent<Image>();

        buttonAll.onClick.RemoveAllListeners();
        buttonDPS.onClick.RemoveAllListeners();
        buttonTank.onClick.RemoveAllListeners();
        buttonSupport.onClick.RemoveAllListeners();

        buttonAll.onClick.AddListener(() => OnClickFilter(null));
        buttonDPS.onClick.AddListener(() => OnClickFilter(RoleHero.DPS));
        buttonTank.onClick.AddListener(() => OnClickFilter(RoleHero.Tank));
        buttonSupport.onClick.AddListener(() => OnClickFilter(RoleHero.Support));
    }

    void OnEnable()
    {
        UpdateFilterButtons();

        LoadHeroes();

        AutoLoadFirstHero();
    }

    void OnDisable()
    {
        currentFilter = null;
        currentPreviewHero = null;

        if (expAnimRoutine != null)
        {
            StopCoroutine(expAnimRoutine);
            expAnimRoutine = null;
        }

        if (levelTextAnimRoutine != null)
        {
            StopCoroutine(levelTextAnimRoutine);
            levelTextAnimRoutine = null;
        }
    }

    void OnClickFilter(RoleHero? role)
    {
        currentFilter = role;

        UpdateFilterButtons();

        LoadHeroes();

        AutoLoadFirstHero();
    }

    void UpdateFilterButtons()
    {
        imageAll.sprite =
            !currentFilter.HasValue
            ? selectedSprite
            : normalSprite;

        imageDPS.sprite =
            currentFilter == RoleHero.DPS
            ? selectedSprite
            : normalSprite;

        imageTank.sprite =
            currentFilter == RoleHero.Tank
            ? selectedSprite
            : normalSprite;

        imageSupport.sprite =
            currentFilter == RoleHero.Support
            ? selectedSprite
            : normalSprite;
    }

    void AutoLoadFirstHero()
    {
        var heroes = PlayerInventory.Instance.GetHeroViewList(
            DatabaseManager.Instance.HeroDatabase
        );

        if (heroes == null || heroes.Count == 0)
            return;

        var first = heroes.FirstOrDefault(
            h => !currentFilter.HasValue
            || h.info.role == currentFilter.Value
        );

        if (first != null && first.info != null)
            LoadPreview(first);
    }

    public void LoadHeroes()
    {
        Clear();

        var heroes = PlayerInventory.Instance.GetHeroViewList(
            DatabaseManager.Instance.HeroDatabase
        );

        foreach (var hero in heroes)
        {
            if (currentFilter.HasValue &&
                hero.info.role != currentFilter.Value)
                continue;

            CreateItem(hero);
        }
    }

    public void LoadPreview(HeroViewData hero)
    {
        currentPreviewHero = hero;

        ClearContentPreview();

        Instantiate(hero.info.HeroPreviewPrefabs, contentPreview);

        if (nameHeroText != null)
            nameHeroText.text = hero.info.Name;

        LoadSpeedBonus(hero);

        LoadSpeedFoodItem(hero.info);

        UpdateSpeedLevelBarInstant();
    }

    void LoadSpeedBonus(HeroViewData hero)
    {
        int currentLevel =
            hero.instance != null
            ? hero.instance.speedLevel
            : 0;

        speed.text = $"Speed Bonus: {currentLevel * 3}";
    }

    void LoadSpeedFoodItem(HeroInfo hero)
    {
        ClearContentSpeedFood();

        if (hero == null ||
            hero.speedFoodList == null ||
            hero.speedFoodList.Count == 0)
            return;

        foreach (int itemId in hero.speedFoodList)
        {
            ItemData itemData =
                DatabaseManager.Instance.ItemDatabase.GetItem(itemId);

            int currentQuantity =
                PlayerInventory.Instance.GetItemQuantity(itemId);

            GameObject go =
                Instantiate(speedFoodPrefab, contentSpeedFood);

            UI_SpeedFoodItem ui =
                go.GetComponent<UI_SpeedFoodItem>();

            ui.Setup(
                itemData,
                currentQuantity,
                OnSpeedFoodClicked
            );
        }
    }

    void OnSpeedFoodClicked(ItemData item)
    {
        if (currentPreviewHero == null ||
            currentPreviewHero.instance == null)
            return;

        if (HeroUpgradeService.Instance == null ||
            HeroUpgradeService.Instance.SpeedConfig == null)
            return;

        var hero = currentPreviewHero.instance;

        int prevLevel = hero.speedLevel;
        int prevExp = hero.currentSpeedExp;

        bool ok =
            HeroUpgradeService.Instance.FeedSpeedExp(hero, item);

        if (!ok) return;

        int newLevel = hero.speedLevel;
        int newExp = hero.currentSpeedExp;

        bool leveledUp = newLevel > prevLevel;

        UpdateSpeedLevelText(newLevel);

        if (leveledUp)
            PlayLevelTextAnim();

        if (expAnimRoutine != null)
            StopCoroutine(expAnimRoutine);

        expAnimRoutine = StartCoroutine(
            AnimateSpeedExpChange(
                prevLevel,
                prevExp,
                newLevel,
                newExp
            )
        );

        LoadSpeedBonus(currentPreviewHero);

        LoadSpeedFoodItem(currentPreviewHero.info);
    }

    void UpdateSpeedLevelBarInstant()
    {
        if (currentLevelText == null ||
            currentLevelExpBarText == null)
            return;

        if (currentPreviewHero == null ||
            currentPreviewHero.instance == null ||
            HeroUpgradeService.Instance == null ||
            HeroUpgradeService.Instance.SpeedConfig == null)
        {
            currentLevelText.text = "-";
            currentLevelExpBarText.text = "-";

            if (expFillImage != null)
                expFillImage.fillAmount = 0f;

            return;
        }

        var hero = currentPreviewHero.instance;

        int level = hero.speedLevel;
        int currentExp = hero.currentSpeedExp;

        UpdateSpeedLevelText(level);

        var config = HeroUpgradeService.Instance.SpeedConfig;

        int idx = level - 1;

        if (idx >= 0 && idx < config.expPerLevel.Length)
        {
            int needExp = config.expPerLevel[idx];

            currentLevelExpBarText.text =
                $"{currentExp} / {needExp}";

            if (expFillImage != null)
            {
                expFillImage.fillAmount =
                    needExp > 0
                    ? (float)currentExp / needExp
                    : 0f;
            }
        }
        else
        {
            currentLevelExpBarText.text = "MAX";

            if (expFillImage != null)
                expFillImage.fillAmount = 1f;
        }
    }

    void UpdateSpeedLevelText(int level)
    {
        if (currentLevelText != null)
            currentLevelText.text = $"Lv. {level}";
    }

    private void PlayLevelTextAnim()
    {
        if (currentLevelText == null)
            return;

        if (levelTextAnimRoutine != null)
            StopCoroutine(levelTextAnimRoutine);

        levelTextAnimRoutine = StartCoroutine(
            AnimateTMP(
                currentLevelText,
                0.1f,
                1.5f,
                Color.green
            )
        );
    }

    private IEnumerator AnimateTMP(
        TextMeshProUGUI txt,
        float duration,
        float scaleUp,
        Color highlightColor)
    {
        if (txt == null)
            yield break;

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

            t.localScale =
                Vector3.Lerp(
                    baseScale,
                    baseScale * scaleUp,
                    eased
                );

            txt.color =
                Color.Lerp(
                    baseColor,
                    highlightColor,
                    eased
                );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;

            float k = Mathf.Clamp01(elapsed / half);
            float eased = Mathf.SmoothStep(0f, 1f, k);

            t.localScale =
                Vector3.Lerp(
                    baseScale * scaleUp,
                    baseScale,
                    eased
                );

            txt.color =
                Color.Lerp(
                    highlightColor,
                    baseColor,
                    eased
                );

            yield return null;
        }

        t.localScale = baseScale;
        txt.color = baseColor;

        levelTextAnimRoutine = null;
    }

    IEnumerator AnimateSpeedExpChange(
        int prevLevel,
        int prevExp,
        int newLevel,
        int newExp)
    {
        var config = HeroUpgradeService.Instance.SpeedConfig;
        int[] table = config.expPerLevel;

        bool GetNeed(int lvl, out int need, out bool isMax)
        {
            int idx = lvl - 1;

            if (idx >= 0 && idx < table.Length)
            {
                need = table[idx];
                isMax = false;
                return true;
            }

            need = 0;
            isMax = true;
            return false;
        }

        bool prevIsMax;
        int prevNeed;

        GetNeed(prevLevel, out prevNeed, out prevIsMax);

        bool newIsMax;
        int newNeed;

        GetNeed(newLevel, out newNeed, out newIsMax);

        if (prevIsMax && newIsMax)
        {
            SetFillInstant(1f);
            SetExpTextMax();
            yield break;
        }

        if (prevLevel == newLevel && !prevIsMax)
        {
            float from =
                prevNeed > 0
                ? (float)prevExp / prevNeed
                : 0f;

            float to =
                newNeed > 0
                ? (float)newExp / newNeed
                : 0f;

            yield return AnimateFillAndText(
                from,
                to,
                prevLevel,
                prevNeed,
                newExp,
                newNeed
            );

            yield break;
        }

        if (!prevIsMax)
        {
            float fromPrev =
                prevNeed > 0
                ? (float)prevExp / prevNeed
                : 0f;

            yield return AnimateFillAndText(
                fromPrev,
                1f,
                prevLevel,
                prevNeed,
                prevNeed,
                prevNeed
            );
        }

        yield return new WaitForSeconds(0.12f);

        if (newIsMax)
        {
            SetFillInstant(1f);
            SetExpTextMax();
            yield break;
        }

        float target =
            newNeed > 0
            ? (float)newExp / newNeed
            : 0f;

        SetFillInstant(0f);

        yield return AnimateFillAndText(
            0f,
            target,
            newLevel,
            newNeed,
            newExp,
            newNeed
        );
    }

    IEnumerator AnimateFillAndText(
        float from,
        float to,
        int levelForText,
        int needForText,
        int displayExp,
        int needForDisplay)
    {
        float duration = 0.1f;
        float elapsed = 0f;

        SetFillInstant(from);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float val =
                Mathf.Lerp(
                    from,
                    to,
                    Mathf.SmoothStep(0f, 1f, t)
                );

            if (expFillImage != null)
                expFillImage.fillAmount = val;

            int shownExp =
                Mathf.RoundToInt(val * needForDisplay);

            if (currentLevelExpBarText != null)
            {
                currentLevelExpBarText.text =
                    $"{shownExp} / {needForDisplay}";
            }

            yield return null;
        }

        if (expFillImage != null)
            expFillImage.fillAmount = to;

        if (currentLevelExpBarText != null)
        {
            currentLevelExpBarText.text =
                $"{displayExp} / {needForDisplay}";
        }
    }

    void SetFillInstant(float v)
    {
        if (expFillImage != null)
            expFillImage.fillAmount = Mathf.Clamp01(v);
    }

    void SetExpTextMax()
    {
        if (currentLevelExpBarText != null)
            currentLevelExpBarText.text = "MAX";
    }

    void CreateItem(HeroViewData data)
    {
        var go =
            Instantiate(heroItemPrefab, contentListHero);

        go.GetComponent<UI_RestaurantHeroItem>()
            .Setup(data, this);
    }

    void ClearContentSpeedFood()
    {
        foreach (Transform child in contentSpeedFood)
            Destroy(child.gameObject);
    }

    void Clear()
    {
        foreach (Transform child in contentListHero)
            Destroy(child.gameObject);
    }

    void ClearContentPreview()
    {
        foreach (Transform child in contentPreview)
            Destroy(child.gameObject);
    }

    public Transform InventoryContent => contentListHero;
}