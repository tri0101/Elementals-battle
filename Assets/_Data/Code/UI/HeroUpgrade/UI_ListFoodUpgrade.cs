using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_ListFoodUpgrade : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform content;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabItem1;   // dùng cho 50,51
    [SerializeField] private GameObject prefabItem2;   // dùng cho 52,53
    [SerializeField] private GameObject prefabItem3;   // dùng cho 54
    [SerializeField] private GameObject prefabItem55;  // dùng cho 55
    [SerializeField] private UI_ListHeroUpgrade uiHeroUpgrade;

    [Header("Level Bar")]
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI currentLevelExpBarText;
    [SerializeField] private Image expFillImage;

    Coroutine expAnimRoutine;
    private Coroutine levelTextAnimRoutine;

    void OnEnable()
    {
        LoadFoodItems();
        UpdateLevelBarInstant();
    }

    void LoadFoodItems()
    {
        Clear();

        CreateItem(50);
        CreateItem(51);
        CreateItem(52);
        CreateItem(53);
        CreateItem(54);
        CreateItem(55);
    }

    void CreateItem(int itemId)
    {
        ItemData itemData = itemDatabase.GetItem(itemId);
        if (itemData == null) return;

        int amount = GetPlayerItemAmount(itemId);
        GameObject prefab = GetPrefabByItemId(itemId);

        if (prefab == null) return;

        var go = Instantiate(prefab, content);
        var ui = go.GetComponent<UI_FoodUpgradeItem>();
        ui.Setup(itemData, amount, OnFoodClicked);
    }

    void OnFoodClicked(ItemData item)
    {
        if (HeroUpgradeContext.SelectedHero == null)
            return;

        var selected = HeroUpgradeContext.SelectedHero.instance;
        var config = HeroUpgradeService.Instance?.LevelConfig;
        if (config == null)
        {
            // Nếu không có config thì fallback về hành vi cũ
            bool okNoCfg = HeroUpgradeService.Instance.FeedExp(selected, item);
            if (okNoCfg) LoadFoodItems();
            return;
        }

        // Lưu trạng thái trước khi nâng
        int prevLevel = selected.level;
        int prevExp = selected.currentExp;

        bool ok = HeroUpgradeService.Instance.FeedExp(selected, item);
        if (!ok) return;

        // Trạng thái sau khi nâng
        int newLevel = selected.level;
        int newExp = selected.currentExp;

        bool leveledUp = newLevel > prevLevel;

        // Update level text + animate highlight if level changed
        UpdateLevelText(newLevel);
        if (leveledUp)
            PlayLevelTextAnim();

        // Hủy animation trước nếu đang chạy
        if (expAnimRoutine != null)
            StopCoroutine(expAnimRoutine);

        // Bắt đầu animation mới cho fillAmount của Image
        expAnimRoutine = StartCoroutine(AnimateExpChange(prevLevel, prevExp, newLevel, newExp));

        LoadFoodItems();

        // Refresh toàn bộ header/chỉ số bên phải; phần "chỉ số" sẽ update theo UI_HeroUpgradeHeader
        if (uiHeroUpgrade != null)
        {
            uiHeroUpgrade.Refresh();

            // Chỉ pop các chỉ số khi lên ít nhất 1 level mới
            if (leveledUp && uiHeroUpgrade.Header != null)
                uiHeroUpgrade.Header.PlayStatUpgradeFx();
        }
    }

    private void PlayLevelTextAnim()
    {
        if (currentLevelText == null) return;

        if (levelTextAnimRoutine != null)
            StopCoroutine(levelTextAnimRoutine);

        levelTextAnimRoutine = StartCoroutine(AnimateTMP(currentLevelText, 0.1f, 1.5f, Color.green));
    }

    private IEnumerator AnimateTMP(TextMeshProUGUI txt, float duration, float scaleUp, Color highlightColor)
    {
        if (txt == null) yield break;

        Transform t = txt.transform;
        Vector3 baseScale = t.localScale;
        Color baseColor = txt.color;

        float half = Mathf.Max(0.0001f, duration * 0.5f);

        // Up
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

        // Down
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
        levelTextAnimRoutine = null;
    }

    IEnumerator AnimateExpChange(int prevLevel, int prevExp, int newLevel, int newExp)
    {
        var config = HeroUpgradeService.Instance.LevelConfig;
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

        // Nếu cả trước và sau đều max level -> hiện MAX
        if (prevIsMax && newIsMax)
        {
            SetFillInstant(1f);
            SetExpTextMax();
            yield break;
        }

        // Nếu không đổi level (vẫn level cũ) và không max -> animate tỉ lệ trong cùng 1 level
        if (prevLevel == newLevel && !prevIsMax)
        {
            float from = prevNeed > 0 ? (float)prevExp / prevNeed : 0f;
            float to = newNeed > 0 ? (float)newExp / newNeed : 0f;
            yield return AnimateFillAndText(from, to, prevLevel, prevNeed, newExp, newNeed);
            yield break;
        }

        // Có level-up xảy ra (có thể tăng nhiều level)
        // 1) animate từ tỷ lệ cũ -> đầy (1.0) cho level cũ (nếu chưa max)
        if (!prevIsMax)
        {
            float fromPrev = prevNeed > 0 ? (float)prevExp / prevNeed : 0f;
            yield return AnimateFillAndText(fromPrev, 1f, prevLevel, prevNeed, prevNeed, prevNeed);
        }

        // pause ngắn để cảm nhận level up
        yield return new WaitForSeconds(0.12f);

        // 2) nếu new là max level -> set full + MAX
        if (newIsMax)
        {
            SetFillInstant(1f);
            SetExpTextMax();
            yield break;
        }

        // 3) animate từ 0 -> tỷ lệ của exp mới trong level mới
        float target = newNeed > 0 ? (float)newExp / newNeed : 0f;
        SetFillInstant(0f);
        yield return AnimateFillAndText(0f, target, newLevel, newNeed, newExp, newNeed);
    }

    // Animate giá trị fillAmount và cập nhật text exp trong khi animate
    IEnumerator AnimateFillAndText(float from, float to, int levelForText, int needForText, int displayExp, int needForDisplay)
    {
        float duration = 0.1f;
        float elapsed = 0f;
        SetFillInstant(from);
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float val = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            if (expFillImage != null) expFillImage.fillAmount = val;

            int shownExp = Mathf.RoundToInt(val * needForDisplay);
            currentLevelExpBarText.text = $"{shownExp} / {needForDisplay}";

            yield return null;
        }

        // finalize
        if (expFillImage != null) expFillImage.fillAmount = to;
        currentLevelExpBarText.text = $"{displayExp} / {needForDisplay}";
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

    void UpdateLevelText(int level)
    {
        if (currentLevelText != null)
            currentLevelText.text = $"Lv. {level}";
    }

    // Hiển thị ngay trạng thái hiện tại khi mở panel (không animate)
    void UpdateLevelBarInstant()
    {
        if (currentLevelText == null || currentLevelExpBarText == null)
            return;

        var selected = HeroUpgradeContext.SelectedHero;
        if (selected == null || HeroUpgradeService.Instance == null || HeroUpgradeService.Instance.LevelConfig == null)
        {
            currentLevelText.text = "-";
            currentLevelExpBarText.text = "-";
            if (expFillImage != null) expFillImage.fillAmount = 0f;
            return;
        }

        var hero = selected.instance;
        int level = hero.level;
        int currentExp = hero.currentExp;

        UpdateLevelText(level);

        var config = HeroUpgradeService.Instance.LevelConfig;
        int nextLevelIndex = level - 1;
        if (nextLevelIndex >= 0 && nextLevelIndex < config.expPerLevel.Length)
        {
            int needExp = config.expPerLevel[nextLevelIndex];
            currentLevelExpBarText.text = $"{currentExp} / {needExp}";
            if (expFillImage != null)
                expFillImage.fillAmount = needExp > 0 ? (float)currentExp / needExp : 0f;
        }
        else
        {
            // max level
            SetExpTextMax();
            SetFillInstant(1f);
        }
    }

    GameObject GetPrefabByItemId(int itemId)
    {
        if (itemId == 50 || itemId == 51)
            return prefabItem1;

        if (itemId == 52 || itemId == 53)
            return prefabItem2;

        if (itemId == 54)
            return prefabItem3;

        if (itemId == 55)
            return prefabItem55;

        return null;
    }

    int GetPlayerItemAmount(int itemId)
    {
        return PlayerInventory.Instance != null ? PlayerInventory.Instance.GetItemQuantity(itemId) : 0;
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}