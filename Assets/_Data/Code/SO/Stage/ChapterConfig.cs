using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Chapter/Chapter Config")]
public class ChapterConfig : ScriptableObject
{
    public int chapterID;
    
    [Header("Drop Items")]
    
    public List<ItemStarReward> dropItems0; // index = 0 => 10 sao, index = 1 => 20 sao, index = 2 => 30 sao
    public List<ItemStarReward> dropItems1;
    public List<ItemStarReward> dropItems2;
}
