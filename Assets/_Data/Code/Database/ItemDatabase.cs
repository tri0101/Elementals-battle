using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;
    private Dictionary<int, ItemData> itemDict;
    public void Init()
    {
        if (itemDict != null) return; 

        itemDict = new Dictionary<int, ItemData>();
        foreach (var item in items)
            itemDict[item.id] = item;
    }

    public ItemData GetItem(int id)
    {
        if (itemDict == null) Init();
        return itemDict.TryGetValue(id, out var item) ? item : null;
    }
}