using UnityEngine;
public enum ItemType
{
    ExpFood,        // sách exp
    Equipment,      // kiếm, khiên
    StarMaterial,   // vật liệu tăng sao
    RankPotion,     // thuốc tăng rank
    Currency,        // vàng, kim cương
    HeroShard
}

[System.Serializable]
public class ItemInstance
{
    public int itemId;
    public int quantity;
}
