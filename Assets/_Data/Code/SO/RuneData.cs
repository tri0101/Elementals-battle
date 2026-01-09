using System.Collections.Generic;
using UnityEngine;

public enum RuneStat
{
    Attack,
    HP,
    Armor,
    
}

[CreateAssetMenu(menuName = "Rune/RuneData")]
public class RuneData : ScriptableObject
{
    public int id;
    public string runeName;

    public List<RuneStatValue> stats;

    public int level;
    public Sprite icon;
}
