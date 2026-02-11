using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_PanelDetailStage : MonoBehaviour
{
    [Header("Database")]
    public StageDatabase stageDatabase;
    public ItemDatabase itemDatabase;
    public HeroDatabase heroDatabase;

    [Header("UI")]
    public Transform contentItem;
    public Transform contentEnemy;
    public Transform starRoot;
    public Transform backButtonSweep;
    public GameObject itemPrefab;
    public GameObject itemPrefabShard;
    public GameObject enemyPrefab;
    public GameObject stage;
    public GameObject backEmpty;
    public Button buttonBack;
    public Button buttonNext;
    public TextMeshProUGUI staminaCost;
    [Header("Star Colors")]
    public Color earnedColor = new Color32(255, 215, 0, 255);      // vàng
    public Color notEarnedColor = new Color32(158, 101, 101, 255); // tối
    [Header("Script")]
    public UI_PanelChooseHero panelChooseHero;
    StageConfig currentStage;
    public StageConfig CurrentStage => currentStage;
    public int currentStageId;
    private int starsEarned;


    public void Awake()
    {
        buttonBack.onClick.AddListener(OnClickBack);

        buttonNext.onClick.AddListener(OnClickNext);
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
        gameObject.SetActive(false);
        panelChooseHero.gameObject.SetActive(true);
        panelChooseHero.SetStageInt(currentStageId);


    }
    public void SetStageInt(int stageId)
    {
        currentStageId = stageId;
    }

}
