using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_PanelChooseHero : MonoBehaviour
{
    [SerializeField] private int currentStageId;

    [Header("Refs")]
    [SerializeField] private HeroGrowthConfig growthConfig;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject heroItemPrefab;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private UI_PanelDetailStage panelDetailStage;
    [SerializeField] private UI_FormationManager formationManager;

    [Header("Buttons")]
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonNext;
    [SerializeField] private Button buttonAll;
    [SerializeField] private Button buttonDPS;
    [SerializeField] private Button buttonTank;
    [SerializeField] private Button buttonSupport;

    [Header("Sprites")]
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;

    private Image imageAll;
    private Image imageDPS;
    private Image imageTank;
    private Image imageSupport;

    private RoleHero? currentFilter = null;

    void Awake()
    {
        imageAll = buttonAll.GetComponent<Image>();
        imageDPS = buttonDPS.GetComponent<Image>();
        imageTank = buttonTank.GetComponent<Image>();
        imageSupport = buttonSupport.GetComponent<Image>();

        buttonBack.onClick.RemoveAllListeners();
        buttonNext.onClick.RemoveAllListeners();
        buttonAll.onClick.RemoveAllListeners();
        buttonDPS.onClick.RemoveAllListeners();
        buttonTank.onClick.RemoveAllListeners();
        buttonSupport.onClick.RemoveAllListeners();

        buttonBack.onClick.AddListener(OnClickBack);
        buttonNext.onClick.AddListener(OnClickNext);

        buttonAll.onClick.AddListener(() => OnClickFilter(null));
        buttonDPS.onClick.AddListener(() => OnClickFilter(RoleHero.DPS));
        buttonTank.onClick.AddListener(() => OnClickFilter(RoleHero.Tank));
        buttonSupport.onClick.AddListener(() => OnClickFilter(RoleHero.Support));
    }

    void OnEnable()
    {
        UpdateFilterButtons();

        LoadHeroes();

        RefreshPower();
    }

    void OnDisable()
    {
        currentFilter = null;
    }

    void OnClickFilter(RoleHero? role)
    {
        currentFilter = role;

        UpdateFilterButtons();

        LoadHeroes();
    }

    void UpdateFilterButtons()
    {
        imageAll.sprite =
            !currentFilter.HasValue
            ? selectedSprite
            : normalSprite;

        imageDPS.sprite =
            currentFilter == RoleHero.DPS
            ? selectedSprite
            : normalSprite;

        imageTank.sprite =
            currentFilter == RoleHero.Tank
            ? selectedSprite
            : normalSprite;

        imageSupport.sprite =
            currentFilter == RoleHero.Support
            ? selectedSprite
            : normalSprite;
    }

    public void LoadHeroes()
    {
        if (formationManager != null &&
            formationManager.IsBusy)
            return;

        Clear();

        var heroes =
            PlayerInventory.Instance.GetHeroViewList(
                DatabaseManager.Instance.HeroDatabase
            );

        foreach (var hero in heroes)
        {
            if (formationManager != null &&
                formationManager.IsHeroInFormation(
                    hero.instance.heroId))
                continue;

            if (currentFilter.HasValue &&
                hero.info.role != currentFilter.Value)
                continue;

            CreateItem(hero);
        }
    }

    public void RefreshPower()
    {
        float totalPower = 0f;

        var allHeroes =
            PlayerInventory.Instance.GetHeroViewList(
                DatabaseManager.Instance.HeroDatabase
            );

        int[] idsInFormation = FormationManager.Load();

        foreach (int id in idsInFormation)
        {
            if (id == -1)
                continue;

            var heroData =
                allHeroes.Find(
                    h => h.instance.heroId == id
                );

            if (heroData != null)
            {
                var stat =
                    HeroStatCalculator.Calculate(
                        heroData.info,
                        heroData.instance,
                        growthConfig
                    );

                totalPower += stat.power;
            }
        }

        powerText.text =
            $"{Mathf.RoundToInt(totalPower)}";
    }

    void CreateItem(HeroViewData data)
    {
        var go =
            Instantiate(heroItemPrefab, content);

        go.GetComponent<UI_HeroChooseItem>()
            .Setup(data, formationManager, this);
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }

    public Transform InventoryContent => content;

    void OnClickBack()
    {
        gameObject.SetActive(false);

        panelDetailStage.gameObject.SetActive(true);

        panelDetailStage.SetStageInt(currentStageId);
    }

    void OnClickNext()
    {
        PlayerInventory.Instance.ConsumeItem(
            3,
            StageContext.selectedStage.staminaCost
        );

        gameObject.SetActive(false);

        GameManager.Instance.LoadAdditiveScene(
            SceneId.BattleScene
        );

        GameManager.Instance.UnLoadAdditiveScene(
            SceneId.MapScene
        );
    }

    private void Update()
    {
        buttonNext.interactable =
            !formationManager.CheckEmpty();
    }

    public void SetStageInt(int stageId)
    {
        currentStageId = stageId;
    }
}