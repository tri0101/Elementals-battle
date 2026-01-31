using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HeroChooseItem : MonoBehaviour
{
    public Button buttonChoose;
    public Image icon;
    public TextMeshProUGUI levelText;
    public Transform starRoot;
    public Transform rankRoot;
    public Image frameRank;
    public HeroViewData Data => data;
    private HeroViewData data;
    private UI_FormationManager formationManager;
    private UI_PanelChooseHero panelChooseHero;

    private HorizontalLayoutGroup starLayout;

    public bool IsInFormation { get; private set; }

    private int blackRank = 1;
    private int greenRank = 5;

    private Color blackColor = Color.black;
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);

    void Awake()
    {
        if (starRoot != null)
            starLayout = starRoot.GetComponent<HorizontalLayoutGroup>();

        buttonChoose.onClick.RemoveAllListeners();
        buttonChoose.onClick.AddListener(OnClickHero);
    }

    public void Setup(
        HeroViewData heroData,
        UI_FormationManager fm,
        UI_PanelChooseHero panel
    )
    {
        data = heroData;
        formationManager = fm;
        panelChooseHero = panel;

        icon.sprite = data.info.iconFace;
        levelText.text = data.instance.level.ToString();

        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank);

        IsInFormation = false;
    }

    void OnClickHero()
    {
        if (!IsInFormation)
        {
            bool ok = formationManager.TryAddHero(this);
            if (!ok)
                Debug.Log("Không thể add hero");
        }
        else
        {
            formationManager.RemoveHero(this, panelChooseHero.InventoryContent);
        }
    }

    public void SetInFormation(bool value)
    {
        IsInFormation = value;
    }


    void UpdateStar(int star)
    {
        if (starLayout != null)
        {
            starLayout.spacing = star <= 4 ? -70f : -25f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                starRoot as RectTransform
            );
        }

        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < star);
    }

    void UpdateRankVisual(int rank)
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
    }
}
