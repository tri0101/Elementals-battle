using UnityEngine;

public class UI_ListHeroUpgrade : MonoBehaviour
{
    
    [SerializeField] private Transform content;
    public Transform Content => content;
    [SerializeField] private GameObject heroUpgradeItemPrefab;
    public GameObject HeroUpgradeItemPrefab => heroUpgradeItemPrefab;
    [SerializeField] private UI_HeroUpgradeHeader header;
    public UI_HeroUpgradeHeader Header => header;
    [SerializeField] private UI_ListRankSourceUpgrade rankSourceList;
    [SerializeField] private UI_ListSkillUpgrade skillUpgradeList;
    [SerializeField] private UI_ListSouls listSouls;

    void OnEnable()
    {
        LoadHeroes();

        // nếu vào từ Inventory
        if (HeroUpgradeContext.SelectedHero != null)
        {
            OnHeroSelected(HeroUpgradeContext.SelectedHero);
        }
    }

    void LoadHeroes()
    {
        Clear();

        var heroes = PlayerInventory.Instance
            .GetHeroViewList(DatabaseManager.Instance.HeroDatabase);

        foreach (var hero in heroes)
            CreateItem(hero);
    }

    void CreateItem(HeroViewData data)
    {
        var go = Instantiate(heroUpgradeItemPrefab, content);

        go.GetComponent<UI_HeroUpgradeItem>()
          .Setup(data, OnHeroSelected);
    }

    void OnHeroSelected(HeroViewData hero)
    {
        HeroUpgradeContext.SelectedHero = hero;
        // Khi click hero → cập nhật panel chi tiết
        UpdateHeroHeader(hero);
        rankSourceList.Setup(hero);
        skillUpgradeList.LoadSkill();
        listSouls.LoadSouls();
    }
    public void UpdateHeroHeader(HeroViewData hero)
    {
        
        header.Setup(hero);
      
    }

    
    public void Refresh()
    {
        
        LoadHeroes();

        
        if (HeroUpgradeContext.SelectedHero != null)
        {
            OnHeroSelected(HeroUpgradeContext.SelectedHero);
        }
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
