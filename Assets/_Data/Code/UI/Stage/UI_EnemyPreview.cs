using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_EnemyPreview : MonoBehaviour
{
    [SerializeField] private Image icon;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Star")]
    [SerializeField] private Transform starRoot;

    [Header("Rank")]
    [SerializeField] private Transform rankRoot;
    [SerializeField] private Image frameRank;

    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;
    private HeroViewData data;
    private Action<HeroViewData> onClickCallback;

    private int blackRank = 1;
    private int greenRank = 5;
    private int blueRank = 9;
    private Color blackColor = new Color(157 / 255f, 143 / 255f, 143 / 255f);
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    private Color blueColor = new Color(0 / 255f, 38 / 255f, 255f / 255f);
    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();
    }
    public void Setup(
    Sprite icon,
    int star,
    int rank
)
    {
        this.icon.sprite = icon;
        UpdateStar(star);
        UpdateRankVisual(rank);
    }
    void UpdateStar(int star)
    {

        if (starLayout != null)
        {
            if (star <= 4)
                starLayout.spacing = -70f;
            else
                starLayout.spacing = -25f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                starRoot as RectTransform
            );
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
        else
        {
            frameRank.color = blueColor;

            int plus = rank - blueRank;
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