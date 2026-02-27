using UnityEngine;

[CreateAssetMenu(menuName = "soul/SoulInfo")]

public class FightSoulInfo : ScriptableObject
{
    public int soulID;
    public FightSoulType fightSoulType;
    public string soulName;
    public Sprite spriteSoul;
    public string description;
}
