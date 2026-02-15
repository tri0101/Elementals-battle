using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StarUpgradeData
{
    public int starLevel;     // Sao sau khi nâng
    public int shardRequired; // Số mảnh cần
    public int goldRequired;  // Số vàng cần
}
[CreateAssetMenu(menuName = "Item/Star/HeroStarConfig")]

public class HeroStarConfig : ScriptableObject
{
    public List<StarUpgradeData> starUpgradeCosts;
}