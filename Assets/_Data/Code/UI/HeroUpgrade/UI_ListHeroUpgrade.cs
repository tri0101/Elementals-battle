using UnityEngine;

public class UI_ListHeroUpgrade : MonoBehaviour
{
    public HeroDatabase heroDatabase;
    public Transform content;
    public GameObject heroUpgradeItemPrefab;
    public UI_HeroUpgradeHeader header;

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
            .GetHeroViewList(heroDatabase);

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
        // Khi click hero → cập nhật panel chi tiết
        header.Setup(hero);
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
