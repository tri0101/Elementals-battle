using UnityEngine;
public enum RoleHero
{
    Tank = 0,
    DPS = 1,
    Support = 2,
}
public enum FightSouldType
{
    GuardianSoul = 0, // hồn bảo hộ
    ReaperSoul = 1, // hồn tử thần
    ArcaneSoul = 2, // hồn ma thuật
    WarHammerSoul = 3, // hồn búa chiến

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
    [Header("Attribute Gereral")]
    public float damage;
    public float health;

    [Header("Attribute Defent")]
    public float armor;
    [Header("Attribute probability")]
    public float criticalRate; // tỉ lệ chí mạng
    public float criticalDamageRate; // sát thương chí mạng (150 , 200)

    [Header("Speed")]
    public float speed;

    [Header("Duration Attack")]
    public float durationA1;
    public float durationA2;

    [Header("UI")]
    public Sprite iconFace;

    [Header("Fight Soul")]
    public FightSouldType fightSouldType;

    [Header("HeroPreview")]
    public GameObject HeroPreviewPrefabs;

    [Header("HeroPrefab")]
    public GameObject HeroPrefab;

}
