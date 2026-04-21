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

        if (content == null || heroItemPrefab == null) return;
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.HeroDatabase == null) return;
        if (PlayerInventory.Instance == null) return;

        var db = DatabaseManager.Instance.HeroDatabase;
        if (db.heroes == null) return;

        // Pass 1: heroes already owned
        foreach (var info in db.heroes)
        {
            if (info == null) continue;
            if (info.ID >= 500) continue;

            var instance = PlayerInventory.Instance.FindHeroInstance(info.ID);
            if (instance == null) continue;

            var go = Instantiate(heroItemPrefab, content);
            var ui = go.GetComponent<UI_HeroItem>();
            if (ui == null) continue;

            ui.Setup(new HeroViewData
            {
                info = info,
                instance = instance
            });
        }

        // Pass 2: heroes not owned (locked) -> push to end
        foreach (var info in db.heroes)
        {
            if (info == null) continue;
            if (info.ID >= 500) continue;

            var instance = PlayerInventory.Instance.FindHeroInstance(info.ID);
            if (instance != null) continue;

            var go = Instantiate(heroItemPrefab, content);
            var ui = go.GetComponent<UI_HeroItem>();
            if (ui == null) continue;

            ui.SetupLocked(info);
        }
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}