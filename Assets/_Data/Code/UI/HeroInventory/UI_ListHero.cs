using System.Collections.Generic;
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

        var owned = new List<(HeroViewData view, float power)>(64);
        var locked = new List<HeroInfo>(64);

        for (int i = 0; i < db.heroes.Count; i++)
        {
            var info = db.heroes[i];
            if (info == null) continue;
            if (info.ID >= 500) continue;

            var instance = PlayerInventory.Instance.FindHeroInstance(info.ID);
            if (instance == null)
            {
                locked.Add(info);
                continue;
            }

            float power = 0f;
            if (DatabaseManager.Instance.HeroGrowthConfig != null)
                power = HeroStatCalculator.Calculate(info, instance, DatabaseManager.Instance.HeroGrowthConfig).power;

            owned.Add((new HeroViewData { info = info, instance = instance }, power));
        }

        owned.Sort((a, b) =>
        {
            int byPower = b.power.CompareTo(a.power);
            if (byPower != 0) return byPower;
            return a.view.info.ID.CompareTo(b.view.info.ID);
        });

        locked.Sort((a, b) => a.ID.CompareTo(b.ID));

        for (int i = 0; i < owned.Count; i++)
        {
            var go = Instantiate(heroItemPrefab, content);
            var ui = go.GetComponent<UI_HeroItem>();
            if (ui == null) continue;

            ui.Setup(owned[i].view);
        }

        for (int i = 0; i < locked.Count; i++)
        {
            var go = Instantiate(heroItemPrefab, content);
            var ui = go.GetComponent<UI_HeroItem>();
            if (ui == null) continue;

            ui.SetupLocked(locked[i]);
        }
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}