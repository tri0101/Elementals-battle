using UnityEngine;

[CreateAssetMenu(menuName = "soul/SoulInfo")]

public class FightSoulInfo : ScriptableObject
{
    public FightSouldType fightSouldType;
    public string soulName;
    public Sprite spriteSoul;
    public string description;
}
