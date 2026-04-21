using UnityEngine;

public class UI_ListHeroUpgrade : MonoBehaviour
{
    [SerializeField] private Transform content;
    public Transform Content => content;
    [SerializeField] private GameObject heroUpgradeItemPrefab;
    public GameObject HeroUpgradeItemPrefab => heroUpgradeItemPrefab;
    [SerializeField] private UI_HeroUpgradeHeader header;
    public UI_HeroUpgradeHeader Header => header;
    [SerializeField] private UI_HeroPreviewHeader previewHeader;
    public UI_HeroPreviewHeader PreviewHeader => previewHeader;
    [SerializeField] private UI_ListRankSourceUpgrade rankSourceList;
    [SerializeField] private UI_ListSkillUpgrade skillUpgradeList;
    [SerializeField] private UI_StarUpgrade starUpgradePanel;
    [SerializeField] private UI_InfoUpgrade infoUpgradePanel;
    [SerializeField] private UI_ListSouls listSouls;

    void OnEnable()
    {
        LoadHeroes();

        if (HeroUpgradeContext.SelectedHero == null)
            return;

        if (HeroUpgradeContext.Mode == HeroUpgradeContext.OpenMode.Upgrade)
        {
            OnHeroSelected(HeroUpgradeContext.SelectedHero);
        }
        else
        {
            // Preview: instance=null, dùng info để setup preview header
            OnHeroPreviewSelected(HeroUpgradeContext.SelectedHero.info);
        }
    }

    void LoadHeroes()
    {
        Clear();

        if (content == null || heroUpgradeItemPrefab == null) return;
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.HeroDatabase == null) return;
        if (PlayerInventory.Instance == null) return;

        var db = DatabaseManager.Instance.HeroDatabase;
        if (db.heroes == null) return;

        // Pass 1: owned heroes first
        foreach (var info in db.heroes)
        {
            if (info == null) continue;
            if (info.ID >= 500) continue;

            var instance = PlayerInventory.Instance.FindHeroInstance(info.ID);
            if (instance == null) continue;

            var go = Instantiate(heroUpgradeItemPrefab, content);
            var ui = go.GetComponent<UI_HeroUpgradeItem>();
            if (ui == null) continue;

            ui.Setup(new HeroViewData { info = info, instance = instance }, OnHeroSelected);
        }

        // Pass 2: locked heroes after
        foreach (var info in db.heroes)
        {
            if (info == null) continue;
            if (info.ID >= 500) continue;

            var instance = PlayerInventory.Instance.FindHeroInstance(info.ID);
            if (instance != null) continue;

            var go = Instantiate(heroUpgradeItemPrefab, content);
            var ui = go.GetComponent<UI_HeroUpgradeItem>();
            if (ui == null) continue;

            ui.SetupLocked(info, OnHeroPreviewSelected);
        }
    }

    void OnHeroSelected(HeroViewData hero)
    {
        HeroUpgradeContext.SelectedHero = hero;
        HeroUpgradeContext.Mode = HeroUpgradeContext.OpenMode.Upgrade;
        GetComponentInParent<HeroUpgradeScene>()?.ApplyMode();

        if (header != null) header.Setup(hero);

        rankSourceList.Setup(hero);
        skillUpgradeList.LoadSkill();
        starUpgradePanel.RefreshUI(hero);
        infoUpgradePanel.RefreshUI(hero);
        listSouls.LoadSouls();
    }

    void OnHeroPreviewSelected(HeroInfo info)
    {
        HeroUpgradeContext.SelectedHero = new HeroViewData
        {
            info = info,
            instance = null
        };
        HeroUpgradeContext.Mode = HeroUpgradeContext.OpenMode.Preview;
        GetComponentInParent<HeroUpgradeScene>()?.ApplyMode();

        if (previewHeader != null) previewHeader.Setup(info);
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