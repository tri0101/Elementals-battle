using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_HeroBattle : MonoBehaviour
{
    public Image icon;

    [Header("Role")]
    public TextMeshProUGUI roleText;

    [Header("Star")]
    public Transform starRoot;

    [Header("Rank")]
    public Transform rankRoot;
    public Image frameRank;

    public Button button;
    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;
    private HeroViewData data;
    private Action<HeroViewData> onClickCallback;

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();
    }
    public void Setup(
        HeroViewData heroData,
        Action<HeroViewData> onClick = null
    )
    {
        data = heroData;
        onClickCallback = onClick;

        // ===== ICON =====
        icon.sprite = data.info.iconFace;

        // ===== ROLE =====
        roleText.text = $"{data.info.role}";

        // ===== STAR + RANK =====
        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

        // ===== CLICK =====
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void UpdateStar(int star)
    { 
        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }


    void UpdateRankVisual(int rank)
    {
        if (rankRoot == null)
            return;

        for (int i = 0; i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(false);

        if (rank < greenRank)
        {
            frameRank.color = blackColor;

            int plus = rank - blackRank;
            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }
        else
        {
            frameRank.color = greenColor;

            int plus = rank - greenRank;
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

    void OnClick()
    {
        onClickCallback?.Invoke(data);
    }
}