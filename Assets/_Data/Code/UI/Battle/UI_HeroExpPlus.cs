using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UI_HeroExpPlus : MonoBehaviour, IObserver
{
    [Header("Icon")]
    public Image icon;
    [Header("Text")]
    public TextMeshProUGUI expPlus;


    [Header("Star")]
    public Transform starRoot;

    [Header("Rank")]
    public Transform rankRoot;
    public Image frameRank;


    // ===== Layout =====
    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;

    // ===== Data =====
    private HeroViewData data;


    // ===== Rank config =====
    private int blackRank = 1;
    private int greenRank = 5;
    private int blueRank = 9;
    private int yellowRank = 13;
    private Color blackColor = new Color(157f/255f, 143f/255f, 143f/255f);
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    private Color blueColor = new Color(0 / 255f, 38 / 255f, 255f / 255f);
    private Color yellowColor = new Color(1f, 1f, 0f);


    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();

        
    }

 
    public void Setup(
        HeroViewData heroData
    )
    {
        data = heroData;
        if (icon != null)
            icon.sprite = data.info.iconFace;
        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

 
    }

 
    public void OnNotify(HeroNotifyType type, object value)
    {
      
    }


    void UpdateStar(int star)
    {
        if (starRoot == null) return;

        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateRankVisual(int rank)
    {
        if (rankRoot == null || frameRank == null)
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
        else if(rank < yellowRank)
        {
            frameRank.color = blueColor;
            int plus = rank - blueRank;
            for (int i = 0; i < plus && i < rankRoot.childCount; i++)
                rankRoot.GetChild(i).gameObject.SetActive(true);
        }
        else
        {
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

            rankLayout.spacing =
                activeCount == 2 ? -200f :
                activeCount == 3 ? -70f : 0f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rankRoot as RectTransform);
        }
    }
    public void SetExpPlus(int exp)
    {
        if (expPlus != null)
        {
            expPlus.text = $"EXP + {exp}";
        }
    }
}