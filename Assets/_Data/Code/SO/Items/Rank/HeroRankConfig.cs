using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Rank/HeroRankConfig")]
public class HeroRankConfig : ScriptableObject
{
    public List<RankRequirement> rankRequirements;
}
