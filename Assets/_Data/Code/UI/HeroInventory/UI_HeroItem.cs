using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HeroItem : MonoBehaviour
{
    public Button buttonUpgrade;
    public Image icon;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameHero;
    public Transform starRoot;
    public Transform rankRoot;
    public Image frameRank;
    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;
    private HeroViewData data;

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = new Color(157/255f, 143/255f, 143/255f);
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();
        if (rankRoot != null)
            rankLayout = rankRoot.GetComponent<HorizontalLayoutGroup>();
    }
    public void Setup(HeroViewData heroData)
    {
        data = heroData;
        // ===== ICON =====
        icon.sprite = data.info.iconFace;

        // ===== LEVEL =====
        levelText.text = data.instance.level.ToString();

        // ===== STAR + RANK =====
        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank, data.info.Name);

        // ===== CLICK =====
        buttonUpgrade.onClick.RemoveAllListeners();
        buttonUpgrade.onClick.AddListener(OnClickUpgrade);
    }

    void OnClickUpgrade()
    {
        
        HeroUpgradeContext.SelectedHero = data;

        
        GameManager.Instance.LoadAdditiveScene(SceneId.HeroUpgradeScene); // = 4
        GameManager.Instance.UnLoadAdditiveScene(SceneId.HeroManagerScene);
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


    void UpdateRankVisual(int rank, string heroName)
    {
        for (int i = 0; i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(false);

        Color rankColor;
        int plusValue;

        if (rank < greenRank)
        {
            rankColor = blackColor;
            frameRank.color = blackColor;
            plusValue = Mathf.Max(0, rank - blackRank);
        }
        else
        {
            rankColor = greenColor;
            frameRank.color = greenColor;
            plusValue = Mathf.Max(0, rank - greenRank);
        }
        
        for (int i = 0; i < plusValue && i < rankRoot.childCount; i++)
            rankRoot.GetChild(i).gameObject.SetActive(true);
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
        if (rank < greenRank) nameHero.color = Color.white;
        else nameHero.color = rankColor;

        nameHero.text = plusValue > 0 ? $"{heroName} +{plusValue}" : heroName;
    }
}
