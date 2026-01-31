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
    public GameObject itemPrefab;
    public GameObject itemPrefabShard;
    public GameObject enemyPrefab;
    public GameObject stage;
    public GameObject panelChooseHero;
    public GameObject backEmpty;
    public Button buttonBack;
    public Button buttonNext;

    public TextMeshProUGUI staminaCost;
    StageConfig currentStage;


    public void Awake()
    {
        buttonBack.onClick.AddListener(OnClickBack);

        buttonNext.onClick.AddListener(OnClickNext);
    }
    public void OnLoadUI(int stageId)
    {
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
    }

    void LoadItems()
    {
        ClearItems();

        foreach (var drop in currentStage.dropItems)
        {
            ItemData itemData = itemDatabase.GetItem(drop.itemId);
            if (itemData == null) continue;

            CreateItem(itemData, drop);
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
    void CreateItem(ItemData data, DropItemData drop)
    {
        GameObject prefab =
            data.type == ItemType.HeroShard
            ? (itemPrefabShard ?? itemPrefab)
            : itemPrefab;

        var go = Instantiate(prefab, contentItem);
        var ui = go.GetComponent<UI_DropPreviewItem>();
        if (ui != null)
            ui.Setup(data, drop);
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
        panelChooseHero.SetActive(true);
        
    }
}
