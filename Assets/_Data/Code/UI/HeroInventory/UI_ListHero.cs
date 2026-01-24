using UnityEngine;

public class UI_ListHero : MonoBehaviour
{
    public HeroDatabase heroDatabase;
    public Transform content;
    public GameObject heroItemPrefab;

    void OnEnable()
    {
        LoadHeroes();
    }

    void LoadHeroes()
    {
        Clear();

        var heroes = PlayerInventory.Instance.GetHeroViewList(heroDatabase);

        foreach (var hero in heroes)
            CreateItem(hero);
    }

    void CreateItem(HeroViewData data)
    {
        var go = Instantiate(heroItemPrefab, content);
        go.GetComponent<UI_HeroItem>().Setup(data);
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
