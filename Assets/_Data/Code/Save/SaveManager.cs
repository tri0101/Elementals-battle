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


public class SaveManager : MonoBehaviour, IObserver
{
    public static SaveManager Instance;
    const string ITEMS_SAVE_KEY = "ItemSaveData";
    const string HEROES_SAVE_KEY = "HeroSaveData";


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

        PlayerInventory.Instance.AddObserver(this);
        HeroUpgradeService.Instance.AddObserver(this);
        LoadInventoryItems();
        LoadInventoryHeroes();
    }

    public void SaveInventory()
    {
        SaveItem();
        SaveHero();
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
    public void OnNotify()
    {
        SaveInventory();
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

    // / ================= DEV TOOL =================
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
