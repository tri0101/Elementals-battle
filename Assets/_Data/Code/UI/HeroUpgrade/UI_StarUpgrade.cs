using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StarUpgrade : MonoBehaviour, IObserver
{
    [Header("Config")]
    [SerializeField] private HeroStarConfig heroStarConfig;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI amountTextCoin;

    [Header("Level Bar")]
    [SerializeField] private TextMeshProUGUI shardBarText;
    [SerializeField] private Image expFillImage;

    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;

    [Header("Refs")]
    [SerializeField] private UI_ListHeroUpgrade uiHeroUpgrade;          // để clone UI trước/sau + tính power
    [SerializeField] private Transform panelUpgradeSuccess;             // object có UI_HeroUpSuccess

    private const int CoinItemId = 1;

    private HeroViewData currentHero;
    private StarUpgradeData nextStarData;
    
    void OnDisable()
    {
        PlayerInventory.Instance.RemoveObbserver(this);
    }
    private void OnEnable()
    {
        PlayerInventory.Instance.AddObserver(this);
        LoadFromContext();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }
    }

    private void LoadFromContext()
    {
        currentHero = HeroUpgradeContext.SelectedHero;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentHero == null || currentHero.instance == null || heroStarConfig == null)
        {
            SetShardUI(0, 0);
            SetCoinUI(0);
            if (upgradeButton != null) upgradeButton.interactable = false;
            return;
        }

        int currentStar = currentHero.instance.star;
        nextStarData = GetNextStarData(currentStar);

        if (nextStarData == null)
        {
            // Max star
            SetShardUI(0, 0);
            SetCoinUI(0);
            if (upgradeButton != null) upgradeButton.interactable = false;
            return;
        }

        // shard is stored in inventory as itemId = heroId + 1000 (same convention as AddHero)
        int shardItemId = currentHero.instance.heroId + 1000;
        int ownedShard = PlayerInventory.Instance != null ? PlayerInventory.Instance.GetItemQuantity(shardItemId) : 0;

        SetShardUI(ownedShard, nextStarData.shardRequired);
        SetCoinUI(nextStarData.goldRequired);

        bool hasShard = ownedShard >= nextStarData.shardRequired;
        bool hasCoin = PlayerInventory.Instance != null &&
                       PlayerInventory.Instance.GetItemQuantity(CoinItemId) >= nextStarData.goldRequired;

        if(!hasCoin)
        if (upgradeButton != null)
            upgradeButton.interactable = hasShard ;
    }

    private void SetShardUI(int currentShard, int needShard)
    {
        if (shardBarText != null)
            shardBarText.text = $"{currentShard}/{needShard}";

        if (expFillImage != null)
        {
            float fill = needShard > 0 ? Mathf.Clamp01((float)currentShard / needShard) : 0f;
            expFillImage.fillAmount = fill;
        }
    }

    private void SetCoinUI(int coinNeed)
    {
        if (amountTextCoin == null) return;

        amountTextCoin.text = coinNeed.ToString();

        if (PlayerInventory.Instance != null && PlayerInventory.Instance.GetItemQuantity(CoinItemId) < coinNeed)
            amountTextCoin.color = Color.red;
        else
            amountTextCoin.color = Color.white;
    }

    private StarUpgradeData GetNextStarData(int currentStar)
    {
        if (heroStarConfig == null || heroStarConfig.starUpgradeCosts == null)
            return null;

        int nextStar = currentStar + 1;
        return heroStarConfig.starUpgradeCosts.Find(x => x.starLevel == nextStar);
    }

    private void OnUpgradeClicked()
    {
        if (currentHero == null || currentHero.instance == null) return;
        if (nextStarData == null) return;
        if (PlayerInventory.Instance == null) return;

        int shardItemId = currentHero.instance.heroId + 1000;
        int ownedShard = PlayerInventory.Instance.GetItemQuantity(shardItemId);
        int ownedCoin = PlayerInventory.Instance.GetItemQuantity(CoinItemId);

        if (ownedCoin < nextStarData.goldRequired)
        {
            UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyCoin();
            return;
        }

        if (ownedShard < nextStarData.shardRequired)
        {
            RefreshUI();
            return;
        }

        // ===== Snapshot before =====
        HeroInstance prevSnapshot = new HeroInstance
        {
            heroId = currentHero.instance.heroId,
            level = currentHero.instance.level,
            currentExp = currentHero.instance.currentExp,
            star = currentHero.instance.star,
            rank = currentHero.instance.rank,
            shard = currentHero.instance.shard
        };

        int prevPower = -1;
        var growth = uiHeroUpgrade != null && uiHeroUpgrade.Header != null ? uiHeroUpgrade.Header.GrowthConfig : null;
        if (growth != null)
        {
            var prevStat = HeroStatCalculator.Calculate(currentHero.info, prevSnapshot, growth);
            prevPower = Mathf.RoundToInt(prevStat.power);
        }

        GameObject prevClone = CloneCurrentHeroItemFromList(disableButton: true);
        if (prevClone != null) prevClone.SetActive(false);

        // ===== Consume resources + upgrade star =====
        bool shardOk = PlayerInventory.Instance.ConsumeItem(shardItemId, nextStarData.shardRequired);
        bool coinOk = PlayerInventory.Instance.ConsumeItem(CoinItemId, nextStarData.goldRequired);

        if (!shardOk || !coinOk)
        {
            // rollback if partial (defensive)
            if (shardOk) PlayerInventory.Instance.AddItem(shardItemId, nextStarData.shardRequired);
            if (coinOk) PlayerInventory.Instance.AddItem(CoinItemId, nextStarData.goldRequired);

            if (prevClone != null) Destroy(prevClone);
            RefreshUI();
            return;
        }

        currentHero.instance.star++;

        // ===== Compute after =====
        int newPower = -1;
        if (growth != null)
        {
            var newStat = HeroStatCalculator.Calculate(currentHero.info, currentHero.instance, growth);
            newPower = Mathf.RoundToInt(newStat.power);
        }

        // Refresh list/header (same pattern as rank upgrade)
        if (uiHeroUpgrade != null)
            uiHeroUpgrade.Refresh();

        GameObject afterClone = null;
        if (uiHeroUpgrade != null && uiHeroUpgrade.HeroUpgradeItemPrefab != null)
        {
            afterClone = Instantiate(uiHeroUpgrade.HeroUpgradeItemPrefab);
            var uiItem = afterClone.GetComponent<UI_HeroUpgradeItem>();
            if (uiItem != null)
                uiItem.Setup(currentHero);

            var btn = afterClone.GetComponent<Button>();
            if (btn != null)
                btn.enabled = false;

            afterClone.SetActive(true);
        }

        // Show success panel (same as UI_ListRankSourceUpgrade)
        if (panelUpgradeSuccess != null)
        {
            var successUI = panelUpgradeSuccess.GetComponent<UI_HeroUpSuccess>();
            if (successUI != null)
            {
                successUI.ShowSucces(prevClone, afterClone, prevPower, newPower);
            }
            else
            {
                panelUpgradeSuccess.gameObject.SetActive(true);
            }
        }

        if (prevClone != null) Destroy(prevClone);
        if (afterClone != null) Destroy(afterClone);

        // Update current panel values (shard/coin/fill/button)
        RefreshUI();
    }

    private GameObject CloneCurrentHeroItemFromList(bool disableButton)
    {
        if (uiHeroUpgrade == null || uiHeroUpgrade.Content == null) return null;
        if (currentHero == null || currentHero.info == null) return null;

        foreach (Transform ch in uiHeroUpgrade.Content)
        {
            var comp = ch.GetComponent<UI_HeroUpgradeItem>();
            if (comp == null || comp.Icon == null) continue;

            if (comp.Icon.sprite == currentHero.info.iconFace)
            {
                var clone = Instantiate(ch.gameObject);
                if (disableButton)
                {
                    var btn = clone.GetComponent<Button>();
                    if (btn != null) btn.enabled = false;
                }
                return clone;
            }
        }

        return null;
    }
    
    public void OnNotify(object data)
    {

        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 1)
                RefreshUI();
        }
    }

}