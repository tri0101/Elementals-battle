using UnityEngine;

public class UI_ListFoodUpgrade : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public Transform content;

    [Header("Prefabs")]
    public GameObject prefabItem1;   // dùng cho 50,51
    public GameObject prefabItem2;   // dùng cho 52,53
    public GameObject prefabItem3;   // dùng cho 54
    public GameObject prefabItem55;  // dùng cho 55

    void OnEnable()
    {
        LoadFoodItems();
    }

    void LoadFoodItems()
    {
        Clear();

        CreateItem(50);
        CreateItem(51);
        CreateItem(52);
        CreateItem(53);
        CreateItem(54);
        CreateItem(55);
    }

    void CreateItem(int itemId)
    {
        ItemData itemData = itemDatabase.GetItem(itemId);
        if (itemData == null) return;

        int amount = GetPlayerItemAmount(itemId);
        GameObject prefab = GetPrefabByItemId(itemId);

        if (prefab == null) return;

        var go = Instantiate(prefab, content);
        go.GetComponent<UI_FoodUpgradeItem>()
          .Setup(itemData, amount);
    }

    GameObject GetPrefabByItemId(int itemId)
    {
        if (itemId == 50 || itemId == 51)
            return prefabItem1;

        if (itemId == 52 || itemId == 53)
            return prefabItem2;

        if (itemId == 54)
            return prefabItem3;

        if (itemId == 55)
            return prefabItem55;

        return null;
    }

    int GetPlayerItemAmount(int itemId)
    {
        var item = PlayerInventory.Instance.items
            .Find(i => i.itemId == itemId);

        return item != null ? item.quantity : 0;
    }

    void Clear()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
