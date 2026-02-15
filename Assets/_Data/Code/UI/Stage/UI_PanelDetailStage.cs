using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_PanelDetailStage : MonoBehaviour, IObserver
{
    [Header("Database")]
    [SerializeField] private StageDatabase stageDatabase;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private HeroDatabase heroDatabase;

    [Header("UI")]
    [SerializeField] private Transform contentItem;
    [SerializeField] private Transform contentEnemy;
    [SerializeField] private Transform starRoot;
    [SerializeField] private Transform backButtonSweep;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPrefabShard;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject stage;
    [SerializeField] private GameObject backEmpty;
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonNext;
    [SerializeField] private TextMeshProUGUI staminaCost;
    [Header("Star Colors")]
    public Color earnedColor = new Color32(255, 215, 0, 255);      // vàng
    public Color notEarnedColor = new Color32(158, 101, 101, 255); // tối
    [Header("Script")]
    [SerializeField] private UI_PanelChooseHero panelChooseHero;
    [SerializeField] private UI_StageSweep stageSweep;
    StageConfig currentStage;
    public StageConfig CurrentStage => currentStage;
    [SerializeField] private int currentStageId;
    private int starsEarned;


    public void Awake()
    {
        buttonBack.onClick.AddListener(OnClickBack);

        buttonNext.onClick.AddListener(OnClickNext);
    }
    void OnEnable()
    {
        PlayerInventory.Instance.AddObserver(this);
    }

    void OnDisable()
    {
        PlayerInventory.Instance.RemoveObbserver(this);
    }
    public void OnLoadUI(int stageId)
    {

        currentStageId = stageId;
        
        
        gameObject.SetActive(true);
        stage.SetActive(false);
        backEmpty.SetActive(true);
        currentStage = stageDatabase.GetStage(stageId);
        if (currentStage == null)
        {
            Debug.LogError($"StageConfig not found: {stageId}");
            return;
        }
        StageContext.selectedStage = currentStage;
        LoadItems();
        LoadEnemy();
        LoadStamina();
        LoadStars();
        LoadButtonSweeps();
    }

    void LoadItems()
    {
        ClearItems();

        foreach (var drop in currentStage.dropItems)
        {
            if(drop.itemId == 1) continue; // skip coin
            ItemData itemData = itemDatabase.GetItem(drop.itemId);
            if (itemData == null) continue;

            CreateItem(itemData);
        }
    }
    void LoadEnemy()
    {
        ClearEnemy();

        foreach (var enemy in currentStage.enemies)
        {
            HeroInfo heroData = heroDatabase.GetHero(enemy.heroId);
            if (heroData == null) continue;

            CreateEnemy(heroData, enemy);
        }
    }
    void LoadStamina()
    {
         if (staminaCost != null)
            staminaCost.text = currentStage.staminaCost.ToString();
        RefreshUIStamina();
    }
    void CreateItem(ItemData data)
    {
        GameObject prefab =
            data.type == ItemType.HeroShard
            ? (itemPrefabShard ?? itemPrefab)
            : itemPrefab;

        var go = Instantiate(prefab, contentItem);
        var ui = go.GetComponent<UI_DropPreviewItem>();
        if (ui != null)
            ui.Setup(data);
    }
    void CreateEnemy(HeroInfo hero, EnemySpawnData enemy)
    {
        var go = Instantiate(enemyPrefab, contentEnemy);
        var ui = go.GetComponent<UI_EnemyPreview>();
        if (ui != null)
            ui.Setup(
                hero.iconFace,
                enemy.star,
                enemy.rank
            );

    }
    void LoadStars()
    {
        for (int i = 0; i < starRoot.childCount; i++)
        {
            Image starImage = starRoot.GetChild(i).GetComponent<Image>();
            if (starImage == null) continue;

            if (i < starsEarned)
                starImage.color = earnedColor;
            else
                starImage.color = notEarnedColor;
        }
    }
    void LoadButtonSweeps()
    {
        if(starsEarned >= 3)
        {
            backButtonSweep.gameObject.SetActive(true);
            backButtonSweep.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            backButtonSweep.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnClick(1));
            backButtonSweep.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            backButtonSweep.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnClick(10));
        }
        else
        {
            backButtonSweep.gameObject.SetActive(false);
   
        }
    }
    public void SetStars(int stars)
    {
        starsEarned = stars;
    }
    void ClearItems()
    {
        for (int i = contentItem.childCount - 1; i >= 0; i--)
            Destroy(contentItem.GetChild(i).gameObject);
    }
    void ClearEnemy()
    {
        for (int i = contentEnemy.childCount - 1; i >= 0; i--)
            Destroy(contentEnemy.GetChild(i).gameObject);
    }
    void OnClickBack()
    {
        gameObject.SetActive(false);
        stage.SetActive(true);
        backEmpty.SetActive(false);
    }
    void OnClickNext()
    {
        if(currentStage.staminaCost > PlayerInventory.Instance.GetItemQuantity(3))
        {
            UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyStamina();
            return;
        }
        else
        {
            PlayerInventory.Instance.ConsumeItem(3, currentStage.staminaCost);
        }
           
        
        gameObject.SetActive(false);
        panelChooseHero.gameObject.SetActive(true);
        panelChooseHero.SetStageInt(currentStageId);


    }
    public void SetStageInt(int stageId)
    {
        currentStageId = stageId;
    }

    void OnClick(int times)
    {
        stageSweep.SetTimes(times);
        stageSweep.gameObject.SetActive(true);
    }
    void RefreshUIStamina()
    {
        if(currentStage.staminaCost <= PlayerInventory.Instance.GetItemQuantity(3))
        {
            staminaCost.color = Color.white;
            buttonNext.interactable = true;
        }
        else
        {
            staminaCost.color = Color.red;
           
        }
    }
    public void OnNotify(object data)
    {

        if (data is ValueTuple<int, int> tuple)
        {
            int itemId = tuple.Item1;
            int value = tuple.Item2;

            if (itemId == 3)
                RefreshUIStamina();
        }
    }
}
