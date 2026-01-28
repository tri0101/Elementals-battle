using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HeroSelect : MonoBehaviour
{
    public Button buttonSelect;
    public Image icon;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameHero;
    public Transform starRoot;
    public Transform rankRoot;
    public Image frameRank;
    private HorizontalLayoutGroup starLayout;
    private HeroViewData data;

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();
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
        buttonSelect.onClick.RemoveAllListeners();
        buttonSelect.onClick.AddListener(OnClickSelect);
    }

    void OnClickSelect()
    {
        
        if (FormationContext.SelectedSlotIndex > 0)
        {
            FormationContext.SelectedHero = data;

            GameManager.Instance.UnLoadAdditiveScene(SceneId.HeroSelectScene);

            return;
        }

        
        HeroUpgradeContext.SelectedHero = data;
        GameManager.Instance.LoadAdditiveScene(SceneId.MainScene);
   
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

        nameHero.color = rankColor;
        nameHero.text = plusValue > 0 ? $"{heroName} +{plusValue}" : heroName;
    }
}
