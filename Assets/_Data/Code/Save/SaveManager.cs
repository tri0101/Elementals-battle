using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

[System.Serializable]
public class DailyTaskProgressData
{
    public List<DailyTaskProgress> dailyTaskProgress = new();
}
public class ProgressInstanceData
{
    public PlayerProgress progress = new();
}
public class TokenExchangeSaveData
{
    public List<TokenExchangeEntry> entries = new();
}
public class LimitedOfferSaveData
{
    public List<LimitedOfferEntry> entries = new();
}
public class GachaPitySaveData
{
    public int standardPityCounter;
    public int featuredPityCounter;
}
public class AccountSaveData
{
    public string playerName;
    public int playerLevel;
    public int playerExp;
}
[System.Serializable]
public class ShopPurchaseStateSaveData
{
    public List<int> currentShopItemIds = new List<int>(16);
    public List<int> soldShopItemIds = new List<int>(16);
}

public class TriaviaQuestionStateSaveData
{
    public List<TriviaQuestionProgress> triviaQuestionProgress = new();
}
public class SaveManager : MonoBehaviour, IObserver
{
    public static SaveManager Instance;
    const string ITEMS_SAVE_KEY = "ItemSaveData";
    const string HEROES_SAVE_KEY = "HeroSaveData";
    const string PROGRESS_SAVE_KEY = "ProgressSaveData";
    const string TOKEN_EXCHANGE_SAVE_KEY = "TokenExchangeSaveData";
    const string LIMITED_OFFER_SAVE_KEY = "LimitedOfferSaveData";
    const string GACHA_PITY_SAVE_KEY = "GachaPitySaveData";
    const string HERO_SELECTEDID_BANNER = "BannerHeroIdSaveData";
    const string MISSON_SAVE_KEY = "MissonSaveData";
    const string SHOP_PURCHASE_STATE_SAVE_KEY = "ShopPurchaseStateSaveData";
    const string TRIVIA_QUESTION_STATE_SAVE_KEY = "TriviaQuestionStateSaveData";
    const string ACCOUNT_SAVE_KEY = "AccountSaveData";
    const string DEFAULT_PLAYER_NAME = "Abc123";

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

        if (TokenExchangeState.Instance != null)
            TokenExchangeState.Instance.AddObserver(this);
        if (LimitedOfferState.Instance != null)
            LimitedOfferState.Instance.AddObserver(this);

        if (GachaManager.Instance != null)
            GachaManager.Instance.AddObserver(this);

        if (AccountManager.Instance != null)
            AccountManager.Instance.AddObserver(this);
        if(DailyTaskManager.Instance != null)
            DailyTaskManager.Instance.AddObserver(this);
         if(ShopPurchaseState.Instance != null)
            ShopPurchaseState.Instance.AddObserver(this);
         if(TriviaQuestionState.Instance != null)
            TriviaQuestionState.Instance.AddObserver(this);


        LoadInventoryItems();
        LoadInventoryHeroes();
        LoadProgress();
        LoadTokenExchange();
        LoadLimitedOffer();
        LoadGachaPity();
        LoadMisson();
        LoadShopPurchaseState();
        LoadTriviaQuestionState();
        LoadAccount();
       
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveAll();
    }

    private void OnApplicationQuit()
    {
        SaveAll();
    }

    private void SaveAll()
    {
        SaveInventory();
        SaveCurrentProgress();
        SaveTokenExchange();
        SaveLimitedOffer();
        SaveGachaPity();
        SaveAccount();
        SaveMisson();
        SaveShopPurchaseState();
        SaveTriviaQuestionState();
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

    void SaveTriviaQuestionState()
    {
        TriaviaQuestionStateSaveData data = new TriaviaQuestionStateSaveData();
        data.triviaQuestionProgress = new List<TriviaQuestionProgress>(TriviaQuestionState.Instance.GetListTriviaQuestion());
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(TRIVIA_QUESTION_STATE_SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    void SaveItem()
    {
        ItemInstanceData data = new ItemInstanceData();
        data.items = new List<ItemInstance>(PlayerInventory.Instance.GetAllItems());

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(ITEMS_SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    void SaveMisson()
    {
 
        DailyTaskProgressData data = new DailyTaskProgressData();
        data.dailyTaskProgress = new List<DailyTaskProgress>(DailyTaskManager.Instance.GetDailyTaskProgress());

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(MISSON_SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    void SaveShopPurchaseState()
    {
        ShopPurchaseStateSaveData data = new ShopPurchaseStateSaveData
        {
            currentShopItemIds = new List<int>(ShopPurchaseState.Instance.GetCurrentShopItemIds()),
            soldShopItemIds = new List<int>(ShopPurchaseState.Instance.GetSoldShopItemIdsCollection())
        };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SHOP_PURCHASE_STATE_SAVE_KEY, json);
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

    void SaveAccount() 
    {
        if (AccountManager.Instance == null)
            return;

        var accI = AccountManager.Instance.AccountI;
        var accP = AccountManager.Instance.AccountP;

        // đảm bảo luôn có tên để lưu
        if (string.IsNullOrWhiteSpace(accI.namePlayer))
            accI.namePlayer = DEFAULT_PLAYER_NAME;

        AccountSaveData data = new AccountSaveData
        {
            playerName = accI.namePlayer,
            playerLevel = accP.level,
            playerExp = accP.exp
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(ACCOUNT_SAVE_KEY, json);
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
    void SaveLimitedOffer()
    {
        if (LimitedOfferState.Instance == null)
            return;

        LimitedOfferSaveData data = LimitedOfferState.Instance.Export();

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LIMITED_OFFER_SAVE_KEY, json);
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
    public void SaveHeroSelectedIdBanner(int heroId)
    {
        PlayerPrefs.SetInt(HERO_SELECTEDID_BANNER, heroId);
        PlayerPrefs.Save();
    }
    public void OnNotify()
    {
        SaveInventory();
        SaveCurrentProgress();
        SaveTokenExchange();
        SaveLimitedOffer();
        SaveGachaPity();
        SaveAccount();
        SaveMisson();
        SaveShopPurchaseState();
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
    void LoadTriviaQuestionState()
    {
        if(!PlayerPrefs.HasKey(TRIVIA_QUESTION_STATE_SAVE_KEY))
            return;
        string json = PlayerPrefs.GetString(TRIVIA_QUESTION_STATE_SAVE_KEY);
        TriaviaQuestionStateSaveData data = JsonUtility.FromJson<TriaviaQuestionStateSaveData>(json);
        if (data == null)
            return;
        if (TriviaQuestionState.Instance != null)
        TriviaQuestionState.Instance.SetListTriviaQuestion(data.triviaQuestionProgress);
    }
    void LoadAccount() 
    {
        if (AccountManager.Instance == null)
            return;

        // Chưa có save => set default name rồi save luôn 1 lần
        if (!PlayerPrefs.HasKey(ACCOUNT_SAVE_KEY))
        {
            AccountManager.Instance.AccountI.namePlayer = DEFAULT_PLAYER_NAME;
            SaveAccount();
            return;
        }

        string json = PlayerPrefs.GetString(ACCOUNT_SAVE_KEY);
        AccountSaveData data = JsonUtility.FromJson<AccountSaveData>(json);

        if (data == null)
            return;

        AccountManager.Instance.AccountI.namePlayer =
            string.IsNullOrWhiteSpace(data.playerName) ? DEFAULT_PLAYER_NAME : data.playerName;

        AccountManager.Instance.SetAccount(new AccountProgress
        {
            level = Mathf.Max(1, data.playerLevel),
            exp = Mathf.Max(0, data.playerExp)
        });
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
    void LoadLimitedOffer()
    {
        if (!PlayerPrefs.HasKey(LIMITED_OFFER_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(LIMITED_OFFER_SAVE_KEY);

        LimitedOfferSaveData data =
            JsonUtility.FromJson<LimitedOfferSaveData>(json);

        if (data == null)
            return;

        if (LimitedOfferState.Instance != null)
            LimitedOfferState.Instance.Import(data);
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
    public int LoadHeroSelectedIdBanner()
    {
        if (!PlayerPrefs.HasKey(HERO_SELECTEDID_BANNER))
            return -1;
        return PlayerPrefs.GetInt(HERO_SELECTEDID_BANNER);

    }
    void LoadMisson()
    {
 

        if (!PlayerPrefs.HasKey(MISSON_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(MISSON_SAVE_KEY);
        DailyTaskProgressData data = JsonUtility.FromJson<DailyTaskProgressData>(json);

        if (data == null)
            return;
        DailyTaskManager.Instance.SetTasks(data.dailyTaskProgress);
    }
    void LoadShopPurchaseState()
    {
        if (!PlayerPrefs.HasKey(SHOP_PURCHASE_STATE_SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(SHOP_PURCHASE_STATE_SAVE_KEY);
        ShopPurchaseStateSaveData data = JsonUtility.FromJson<ShopPurchaseStateSaveData>(json);

        if (data == null)
            return;

        ShopPurchaseState.Instance.SetCurrentShopItemIds(data.currentShopItemIds);

        ShopPurchaseState.Instance.ResetSoldOnly();
        ShopPurchaseState.Instance.SetSoldShopItemIds(data.soldShopItemIds);
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