using UnityEngine;
using UnityEngine.UI;

public class UI_ListInventory : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPrefabShard;
    [SerializeField] private Button buttonAll;
    [SerializeField] private Button buttonResource;
    [SerializeField] private Button buttonShard;
    [SerializeField] private Button buttonOther;

    enum FilterType { All, Resource, Shard, Other }
    FilterType currentFilter = FilterType.All;

    void OnEnable()
    {
        RegisterButtonCallbacks();
        LoadItems();
    }

    void OnDisable()
    {
        UnregisterButtonCallbacks();
    }

    void RegisterButtonCallbacks()
    {
        if (buttonAll != null)
        {
            buttonAll.onClick.RemoveAllListeners();
            buttonAll.onClick.AddListener(() => SetFilter(FilterType.All));
        }

        if (buttonResource != null)
        {
            buttonResource.onClick.RemoveAllListeners();
            buttonResource.onClick.AddListener(() => SetFilter(FilterType.Resource));
        }

        if (buttonShard != null)
        {
            buttonShard.onClick.RemoveAllListeners();
            buttonShard.onClick.AddListener(() => SetFilter(FilterType.Shard));
        }

        if (buttonOther != null)
        {
            buttonOther.onClick.RemoveAllListeners();
            buttonOther.onClick.AddListener(() => SetFilter(FilterType.Other));
        }
    }

    void UnregisterButtonCallbacks()
    {
        if (buttonAll != null) buttonAll.onClick.RemoveAllListeners();
        if (buttonResource != null) buttonResource.onClick.RemoveAllListeners();
        if (buttonShard != null) buttonShard.onClick.RemoveAllListeners();
        if (buttonOther != null) buttonOther.onClick.RemoveAllListeners();
    }

    void SetFilter(FilterType filter)
    {
        currentFilter = filter;
        LoadItems();
    }

    void LoadItems()
    {
        Clear();

        if (PlayerInventory.Instance == null || itemDatabase == null || itemPrefab == null || content == null)
            return;

        foreach (var itemInstance in PlayerInventory.Instance.Items)
        {
            if (itemInstance == null) continue;

            var itemData = itemDatabase.GetItem(itemInstance.itemId);
            if (itemData == null) continue;

          
            if (itemData.type == ItemType.Currency) continue;

            if (!ShouldShowItem(itemData))
                continue;

            CreateItem(itemData, itemInstance.quantity);
        }
    }

    bool ShouldShowItem(ItemData data)
    {
        switch (currentFilter)
        {
            case FilterType.All:
                return true;

            case FilterType.Resource:
              
                return data.type == ItemType.RankSource || data.type == ItemType.ExpFood;

            case FilterType.Shard:
                return data.type == ItemType.HeroShard;

            case FilterType.Other:

                return data.type != ItemType.RankSource
                    && data.type != ItemType.ExpFood
                    && data.type != ItemType.HeroShard;
                  

            default:
                return true;
        }
    }

    void CreateItem(ItemData data, int amount)
    {
        if (data == null || content == null) return;

        GameObject prefabToUse = (data.type == ItemType.HeroShard) ? (itemPrefabShard ?? itemPrefab) : itemPrefab;
        if (prefabToUse == null) return;

        var go = Instantiate(prefabToUse, content);
        var ui = go.GetComponent<UI_InventoryItem>();
        if (ui != null)
            ui.Setup(data, amount);
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}