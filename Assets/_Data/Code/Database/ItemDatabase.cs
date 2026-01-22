using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetItem(int id)
    {
        return items.Find(i => i.id == id);
    }
}
