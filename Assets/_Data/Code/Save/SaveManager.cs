using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class InventorySaveData
{
    public List<ItemInstance> items = new();
}

public class SaveManager : MonoBehaviour, IObserver
{
    public static SaveManager Instance;

    

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
    }

    public void SaveInventory()
    {
        InventorySaveData data = new InventorySaveData();
        data.items = new List<ItemInstance>(PlayerInventory.Instance.GetAllItems());

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("InventorySaveData", json);
        PlayerPrefs.Save();
    }
    public void OnNotify()
    {
        SaveInventory();
    }
    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey("InventorySaveData"))
            return;

        string json = PlayerPrefs.GetString("InventorySaveData");

        InventorySaveData data =
            JsonUtility.FromJson<InventorySaveData>(json);

        PlayerInventory.Instance.SetItems(data.items);
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
