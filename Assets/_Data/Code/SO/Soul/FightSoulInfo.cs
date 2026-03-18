using NUnit.Framework.Constraints;
using UnityEngine;


[System.Serializable]
public class SoulValueConfig
{
    public int value;
    
}
[CreateAssetMenu(menuName = "soul/SoulInfo")]
public class FightSoulInfo : ScriptableObject
{
    public int soulID;
    public FightSoulType fightSoulType;
    public string soulName;
    public Sprite spriteSoul;
    public string description;
    public bool percentValue;
    public SoulValueConfig[] soulValueConfigs;
}
