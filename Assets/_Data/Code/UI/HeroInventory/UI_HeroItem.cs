using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroItem : MonoBehaviour
{
    public Button buttonUpgrade;
    public Image icon;
    public Image backEmpty;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameHero;
    public Transform starRoot;
    public Transform rankRoot;
    public Image frameRank;

    [Header("Button Text")]
    [SerializeField] private TextMeshProUGUI buttonUpgradeText;

    private HorizontalLayoutGroup starLayout;
    private HorizontalLayoutGroup rankLayout;
    private HeroViewData data;
    private HeroInfo lockedInfo;
    private bool isLocked;

    private int blackRank = 1;
    private int greenRank = 5;
    private int blueRank = 9;
    private int yellowRank = 13;
    private Color blackColor = new Color(157 / 255f, 143 / 255f, 143 / 255f);
    private Color greenColor = new Color(73f / 255f, 1f, 115f / 255f);
    private Color blueColor = new Color(0 / 255f, 38f/255f, 255f / 255f);
    private Color yellowColor = new Color(255 / 255f, 215 / 255f, 0 / 255f);

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
        lockedInfo = null;
        isLocked = false;

        if (backEmpty != null) backEmpty.gameObject.SetActive(false);

        icon.sprite = data.info.iconFace;
        levelText.text = data.instance.level.ToString();
        nameHero.text = data.info.Name;

        UpdateStar(data.instance.star);
        UpdateRankVisual(data.instance.rank, data.info.Name);

        SetButtonLabel("Upgrade");

        if (buttonUpgrade != null)
        {
            buttonUpgrade.onClick.RemoveAllListeners();
            buttonUpgrade.onClick.AddListener(OnClickUpgradeOrPreview);
            buttonUpgrade.interactable = true;
        }
    }

    public void SetupLocked(HeroInfo info)
    {
        data = null;
        lockedInfo = info;
        isLocked = true;

        if (backEmpty != null) backEmpty.gameObject.SetActive(true);

        if (icon != null) icon.sprite = info != null ? info.iconFace : null;
        if (nameHero != null) nameHero.text = info != null ? info.Name : string.Empty;

        if (levelText != null) levelText.text = "1";
        UpdateStar(4);
        UpdateRankVisual(blackRank, info != null ? info.Name : string.Empty);

        SetButtonLabel("Preview");

        if (buttonUpgrade != null)
        {
            buttonUpgrade.onClick.RemoveAllListeners();
            buttonUpgrade.onClick.AddListener(OnClickUpgradeOrPreview);
            buttonUpgrade.interactable = true;
        }
    }

    private void SetButtonLabel(string text)
    {
        if (buttonUpgradeText != null)
        {
            buttonUpgradeText.text = text;
            return;
        }

        if (buttonUpgrade != null)
        {
            var tmp = buttonUpgrade.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) tmp.text = text;
        }
    }

    void OnClickUpgradeOrPreview()
    {
        if (isLocked)
        {
            // Preview: SelectedHero vẫn dùng, instance = null
            HeroUpgradeContext.SelectedHero = new HeroViewData
            {
                info = lockedInfo,
                instance = null
            };
            HeroUpgradeContext.Mode = HeroUpgradeContext.OpenMode.Preview;
        }
        else
        {
            HeroUpgradeContext.SelectedHero = data;
            HeroUpgradeContext.Mode = HeroUpgradeContext.OpenMode.Upgrade;
        }
       
        GameManager.Instance.LoadAdditiveScene(SceneId.HeroUpgradeScene);
        GameManager.Instance.UnLoadAdditiveScene(SceneId.HeroManagerScene);
    }

    void UpdateStar(int star)
    {
        if (starLayout != null)
        {
            starLayout.spacing = (star <= 4) ? -70f : -25f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(starRoot as RectTransform);
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
        else if (rank < blueRank)
        {
            rankColor = greenColor;
            frameRank.color = greenColor;
            plusValue = Mathf.Max(0, rank - greenRank);
        }
        else if(rank < yellowRank)
        {
            rankColor = blueColor;
            frameRank.color = blueColor;
            plusValue = Mathf.Max(0, rank - blueRank);
        }
        else
        {
            rankColor = yellowColor;
            frameRank.color = yellowColor;
            plusValue = Mathf.Max(0, rank - yellowRank);
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