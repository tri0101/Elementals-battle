using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_PanelChooseHero : MonoBehaviour
{
    public int currentStageId;
    public HeroDatabase heroDatabase;
    public HeroGrowthConfig growthConfig;
    public Transform content;
    public GameObject heroItemPrefab;
    public TextMeshProUGUI powerText;
    public UI_PanelDetailStage panelDetailStage;
    public UI_FormationManager formationManager;

    [Header("Buttons")]
    public Button buttonBack;
    public Button buttonNext;
    public Button buttonAll;
    public Button buttonDPS;
    public Button buttonTank;
    public Button buttonSupport;

    private RoleHero? currentFilter = null;

    void Awake()
    {
        buttonBack.onClick.AddListener(OnClickBack);
        buttonNext.onClick.AddListener(OnClickNext);

        buttonAll.onClick.AddListener(() => OnClickFilter(null));
        buttonDPS.onClick.AddListener(() => OnClickFilter(RoleHero.DPS));
        buttonTank.onClick.AddListener(() => OnClickFilter(RoleHero.Tank));
        buttonSupport.onClick.AddListener(() => OnClickFilter(RoleHero.Support));
    }

    void OnEnable()
    {
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
        LoadHeroes();
    }

    public void LoadHeroes()
    {
        
        if (formationManager != null && formationManager.IsBusy)
            return;

        Clear();
        var heroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);

        foreach (var hero in heroes)
        {
            if (formationManager != null && formationManager.IsHeroInFormation(hero.instance.heroId))
                continue;

            if (currentFilter.HasValue && hero.info.role != currentFilter.Value)
                continue;

            CreateItem(hero);
        }
    }

    public void RefreshPower()
    {
        float totalPower = 0f;
        var allHeroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);

        int[] idsInFormation = FormationManager.Load();

        foreach (int id in idsInFormation)
        {
            if (id == -1) continue;

            var heroData = allHeroes.Find(h => h.instance.heroId == id);
            if (heroData != null)
            {
                var stat = HeroStatCalculator.Calculate(heroData.info, heroData.instance, growthConfig);
                totalPower += stat.power;
            }
        }

        powerText.text = $"{Mathf.RoundToInt(totalPower)}";
    }

    void CreateItem(HeroViewData data)
    {
        var go = Instantiate(heroItemPrefab, content);
        go.GetComponent<UI_HeroChooseItem>().Setup(data, formationManager, this);
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
        gameObject.SetActive(false);
        StageContext.selectedStage = panelDetailStage.CurrentStage;
        GameManager.Instance.LoadAdditiveScene(SceneId.BattleScene);
        GameManager.Instance.UnLoadAdditiveScene(SceneId.MapScene);

    }

    public void SetStageInt(int stageId) => currentStageId = stageId;
}