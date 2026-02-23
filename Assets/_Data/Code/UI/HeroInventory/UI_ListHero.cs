using UnityEngine;

public class UI_ListHero : MonoBehaviour
{
   
    [SerializeField] private Transform content;
    [SerializeField] private GameObject heroItemPrefab;

    void OnEnable()
    {
        LoadHeroes();
    }

    void LoadHeroes()
    {
        Clear();

        var heroes = PlayerInventory.Instance.GetHeroViewList(DatabaseManager.Instance.HeroDatabase);

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
