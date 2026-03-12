using UnityEngine;
public enum ItemType
{
    ExpFood =0 ,        // sách exp
    RankSource = 1,      // kiếm, khiên
    StarMaterial = 2,   // vật liệu tăng sao
    RankPotion = 3,     // thuốc tăng rank
    Currency = 4,        // vàng, kim cương
    HeroShard = 5,
    Other = 6,         // các loại item khác
    SpeedFood = 7,       // thức ăn tăng tốc độ
}

[System.Serializable]
public class ItemInstance
{
    public int itemId;
    public int quantity;
}
