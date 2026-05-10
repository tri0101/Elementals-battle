using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    public Image Icon => icon;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Star")]
    [SerializeField] private Transform starRoot;

    [Header("Rank")]
    [SerializeField] private Transform rankRoot;
    [SerializeField] private Image frameRank;

    [Header("Empty")]
    [SerializeField] private Image backEmpty;

    [SerializeField] private Button button;
    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;

    private HeroViewData data;
    private Action<HeroViewData> onClickOwned;

    private HeroInfo lockedInfo;
    private Action<HeroInfo> onClickLocked;

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();
    }

    public void Setup(HeroViewData heroData, Action<HeroViewData> onClick = null)
    {
        data = heroData;
        onClickOwned = onClick;

        lockedInfo = null;
        onClickLocked = null;

        if (backEmpty != null)
            backEmpty.gameObject.SetActive(false);

        if (icon != null) icon.sprite = data.info.iconFace;
        if (levelText != null) levelText.text = $"{data.instance.level}";

        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickOwned);
            button.interactable = true;
        }
    }

    public void SetupLocked(HeroInfo info, Action<HeroInfo> onClick = null)
    {
        data = null;
        onClickOwned = null;

        lockedInfo = info;
        onClickLocked = onClick;

        if (backEmpty != null)
            backEmpty.gameObject.SetActive(true);

        if (icon != null)
            icon.sprite = info != null ? info.iconFace : null;

        if (levelText != null)
            levelText.text = "1"; // default

        UpdateStar(4); // default 4 star
        UpdateRankVisual(1);  // default black rank

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickLocked);
            button.interactable = true;
        }
    }

    private void OnClickOwned()
    {
        if (data == null) return;

        onClickOwned?.Invoke(data);
        ReloadPreviewListIfAny();
    }

    private void OnClickLocked()
    {
        if (lockedInfo == null) return;

        onClickLocked?.Invoke(lockedInfo);
        ReloadPreviewListIfAny();
    }

    private void ReloadPreviewListIfAny()
    {
        // UI_ListPreview nằm trong cùng HeroUpgradeScene (parent)
        var preview = GetComponentInParent<HeroUpgradeScene>(true);
        if (preview != null)
        {
            var listPreview = preview.BackPreviewTransform.GetComponent<UI_ListPreview>();
            if(listPreview != null)
            {
                listPreview.RefreshUI(HeroUpgradeContext.SelectedHero);
              
            }
        }
        

    }

    void UpdateStar(int star)
    {
        if (starLayout != null)
        {
            if (star <= 4)
                starLayout.spacing = -70f;
            else
                starLayout.spacing = -25f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(starRoot as RectTransform);
        }

        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateRankVisual(int rank)
    {
        if (rankRoot == null)
            return;

        for (int i = 0; i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(false);

        // Giữ logic cũ của bạn
        int blackRank = 1;
        int greenRank = 5;
        int blueRank = 9;
        int yellowRank = 13;
        Color blackColor = new Color(157 / 255f, 143 / 255f, 143 / 255f);
        Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
        Color blueColor = new Color(0f / 255f, 38/255f, 255f / 255f);
        Color yellowColor = new Color(1f, 1f, 0f);

        if (rank < greenRank)
        {
            frameRank.color = blackColor;

            int plus = rank - blackRank;
            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }
        else if(rank < blueRank)
        {
            frameRank.color = greenColor;

            int plus = rank - greenRank;
            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }
        else if(rank < yellowRank){
            frameRank.color = blueColor;
            int plus = rank - blueRank;
            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }
        else { 
            frameRank.color = yellowColor;
            int plus = rank - yellowRank;
            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }

        if (rankLayout != null)
        {
            int activeCount = 0;
            for (int i = 0; i < rankRoot.childCount; i++)
                if (rankRoot.GetChild(i).gameObject.activeSelf)
                    activeCount++;

            if (activeCount == 2)
                rankLayout.spacing = -200f;
            else if (activeCount == 3)
                rankLayout.spacing = -70f;
            else
                rankLayout.spacing = 0f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rankRoot as RectTransform);
        }
    }
}