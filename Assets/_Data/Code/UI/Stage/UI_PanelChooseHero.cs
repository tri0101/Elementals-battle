using UnityEngine;
using UnityEngine.UI;

public class UI_PanelChooseHero : MonoBehaviour
{
    public int currentStageId;
    public HeroDatabase heroDatabase;
    public Transform content;
    public GameObject heroItemPrefab;
    public UI_PanelDetailStage panelDetailStage; 
    public Button buttonBack;
    public Button buttonNext;
    public UI_FormationManager formationManager;
    public void Awake() {
        buttonBack.onClick.AddListener(OnClickBack); 
        buttonNext.onClick.AddListener(OnClickNext);
    }
    void OnEnable()
    {
        LoadHeroes();
    }

    void LoadHeroes()
    {
        Clear();

        var heroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);

        foreach (var hero in heroes)
        {
            if (formationManager.IsHeroInFormation(hero.info))
                continue;

            CreateItem(hero);
        }
    }


    void CreateItem(HeroViewData data)
    {
        var go = Instantiate(heroItemPrefab, content);
        go.GetComponent<UI_HeroChooseItem>()
          .Setup(data, formationManager, this);
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }

    public Transform InventoryContent => content;
    void OnClickBack() { 
        gameObject.SetActive(false);
        panelDetailStage.gameObject.SetActive(true);
        panelDetailStage.SetStageInt(currentStageId); 
    }
    void OnClickNext() {
        gameObject.SetActive(false);
    }
    public void SetStageInt(int stageId)
    {
        currentStageId = stageId;
    }

}
