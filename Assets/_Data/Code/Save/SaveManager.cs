using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstanceData
{
    public List<ItemInstance> items = new();
}

public class HeroInstanceData
{
    public List<HeroInstance> heroes = new();
}

public class ProgressInstanceData
{
    public PlayerProgress progress = new();
}

public class TokenExchangeSaveData
{
    public List<TokenExchangeEntry> entries = new();
}

public class GachaPitySaveData
{
    public int standardPityCounter;
    public int featuredPityCounter;
}

public class SaveManager : MonoBehaviour, IObserver
{
    public static SaveManager Instance;
    const string ITEMS_SAVE_KEY = "ItemSaveData";
    const string HEROES_SAVE_KEY = "HeroSaveData";
    const string PROGRESS_SAVE_KEY = "ProgressSaveData";
    const string TOKEN_EXCHANGE_SAVE_KEY = "TokenExchangeSaveData";
    const string GACHA_PITY_SAVE_KEY = "GachaPitySaveData";
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.AddObserver(this);

        if (HeroUpgradeService.Instance != null)
            HeroUpgradeService.Instance.AddObserver(this);

        if (ProgressManager.Instance != null)
            ProgressManager.Instance.AddObserver(this);

        if(TokenExchangeState.Instance != null)
            TokenExchangeState.Instance.AddObserver(this);


        if (GachaManager.Instance != null)
            GachaManager.Instance.AddObserver(this);

        LoadInventoryItems();
        LoadInventoryHeroes();
        LoadProgress();
        LoadTokenExchange();
        LoadGachaPity();
    }

    public void SaveInventory()
    {
        SaveItem();
        SaveHero();
    }

    public void SaveCurrentProgress()
    {
        SaveProgress();
    }

    void SaveItem()
    {
        ItemInstanceData data = new ItemInstanceData();
        data.items = new List<ItemInstance>(PlayerInventory.Instance.GetAllItems());

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(ITEMS_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    void SaveHero()
    {
        HeroInstanceData data = new HeroInstanceData();
        data.heroes = new List<HeroInstance>(PlayerInventory.Instance.GetAllHeroes());

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(HEROES_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    void SaveProgress()
    {
        ProgressInstanceData data = new ProgressInstanceData();
        data.progress = ProgressManager.Instance.getProgress();

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PROGRESS_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    void SaveTokenExchange()
    {
        if (TokenExchangeState.Instance == null)
            return;

        TokenExchangeSaveData data = TokenExchangeState.Instance.Export();

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(TOKEN_EXCHANGE_SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    void SaveGachaPity()
    {
        if (GachaManager.Instance == null)
            return;

        var data = new GachaPitySaveData
        {
            standardPityCounter = GachaManager.Instance.StandardPityCounter,
            featuredPityCounter = GachaManager.Instance.FeaturedPityCounter
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GACHA_PITY_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void OnNotify()
    {
        SaveInventory();
        SaveCurrentProgress();
        SaveTokenExchange();
        SaveGachaPity();
    }

    public void LoadInventoryItems()
    {
        if (!PlayerPrefs.HasKey(ITEMS_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(ITEMS_SAVE_KEY);

        ItemInstanceData data =
            JsonUtility.FromJson<ItemInstanceData>(json);

        PlayerInventory.Instance.SetItems(data.items);
    }

    public void LoadInventoryHeroes()
    {
        if (!PlayerPrefs.HasKey(HEROES_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(HEROES_SAVE_KEY);

        HeroInstanceData data =
            JsonUtility.FromJson<HeroInstanceData>(json);

        PlayerInventory.Instance.SetHeroes(data.heroes);
    }

    public void LoadProgress()
    {
        if (!PlayerPrefs.HasKey(PROGRESS_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(PROGRESS_SAVE_KEY);

        ProgressInstanceData data =
            JsonUtility.FromJson<ProgressInstanceData>(json);

        ProgressManager.Instance.SetProgress(data.progress);
    }

    void LoadTokenExchange()
    {
        if (!PlayerPrefs.HasKey(TOKEN_EXCHANGE_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(TOKEN_EXCHANGE_SAVE_KEY);

        TokenExchangeSaveData data =
            JsonUtility.FromJson<TokenExchangeSaveData>(json);

        if (data == null)
            return;

        if (TokenExchangeState.Instance != null)
            TokenExchangeState.Instance.Import(data);
    }
    void LoadGachaPity()
    {
        if (!PlayerPrefs.HasKey(GACHA_PITY_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(GACHA_PITY_SAVE_KEY);
        GachaPitySaveData data = JsonUtility.FromJson<GachaPitySaveData>(json);

        if (data == null)
            return;

        if (GachaManager.Instance != null)
            GachaManager.Instance.SetPityCounters(data.standardPityCounter, data.featuredPityCounter);
    }

#if UNITY_EDITOR
    [ContextMenu("CLEAR ALL SAVE")]
    private void ClearAllSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs deleted.");
    }
#endif
}