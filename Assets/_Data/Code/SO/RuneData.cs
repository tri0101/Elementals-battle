using System.Collections.Generic;
using UnityEngine;

public enum RuneStat
{
    PhysicalDmg,
    MagicalDmg,
    HP,
    PhysicalArmor,
    MagicalArmor,
    CriticalRate,
    CriticalDamageRate,
    CDR,


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
