using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RestaurantHeroItem : MonoBehaviour
{
    [SerializeField] private Button buttonChoose;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform starRoot;
    [SerializeField] private Transform rankRoot;
    [SerializeField] private Image frameRank;

    public HeroViewData Data => data;
    private HeroViewData data;
    private UI_PanelHeroRestaurant panelRestaurant;

    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;



    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = new Color(157 / 255f, 143 / 255f, 143 / 255f);
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();
        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();


        buttonChoose.onClick.RemoveAllListeners();
        buttonChoose.onClick.AddListener(OnClickHero);
    }

    public void Setup(HeroViewData heroData, UI_PanelHeroRestaurant panel)
    {
        data = heroData;
        panelRestaurant = panel;

        icon.sprite = data.info.iconFace;
        levelText.text = data.instance.level.ToString();

        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

    }

    void OnClickHero()
    {
        panelRestaurant.LoadPreview(data);
    }


    void UpdateStar(int star)
    {
        if (starLayout != null)
        {
            starLayout.spacing = star <= 4 ? -70f : -25f;
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
  
}