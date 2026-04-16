
using UnityEngine;
using System.Collections.Generic;
public enum RoleHero
{
    Tank = 0,
    DPS = 1,
    Support = 2,
}
public enum FightSoulType
{
    ManaSoul = 0, // hồn liên quan đến mana
    SkillRateSoul = 1, // hồn liên quan đến tỉ lệ sử dụng kỹ năng
    HealthSoul = 2, // hồn liên quan đến máu
  

}
public enum Tag
{
    Male = 0 , //Nam
    Female = 1, //Nữ
    Warrior = 2, //Chiến binh
    Mage = 3, //Pháp sư
    Assassin = 4, //Sát thủ
    Fighter = 5, //Đấu sĩ
    
}
[CreateAssetMenu(menuName ="hero/heroInfo")]



public class HeroInfo : ScriptableObject
{

    [Header("ID")]
    public int ID;
    public string Name;
    [Header("Role")]
    public RoleHero role;
    [Header("Abilities")]
    public AbilityInfo normalAttack;
    public AbilityInfo skill;
    [Range(0f, 1f)] public float skillChance = 1f;
    public AbilityInfo ultimate;
    public AbilityInfo passive;
    [Header("Special Abilities")]
    public AbilityInfo ultimateSpecial;
    [Header("Attribute Gereral")]
    public float damage;
    public float health;

    [Header("Attribute Defent")]
    public float armor;
    [Header("Attribute probability")]
    public float criticalRate; // tỉ lệ chí mạng
    public float criticalDamageRate; // sát thương chí mạng (150 , 200)
    public float lifeSteal; // hút máu
    [Header("Speed")]
    public float speed;
    [Header("Speed Food")]
    public List<int> speedFoodList; // 0 - tier 1 , // 1 - tier 2 , // 2 - tier 3

    [Header("List Tag")]
    public List<Tag> tags = new List<Tag>(); // list các tag của hero

    [Header("UI")]
    public Sprite iconFace;

    [Header("Fight Soul")]
    public List<int> soulID = new List<int>();

    [Header("HeroPreview")]
    public GameObject HeroPreviewPrefabs;

    [Header("HeroPrefab")]
    public GameObject HeroPrefab;


    
}
