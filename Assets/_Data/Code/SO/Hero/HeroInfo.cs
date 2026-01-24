using UnityEngine;
public enum RoleHero
{
    Tank = 0,
    DPS = 1,
    Support = 2,
}
[CreateAssetMenu(menuName ="hero/heroInfo")]


public class HeroInfo : ScriptableObject
{

    [Header("ID")]
    public int ID;
    public string Name;
    [Header("Role")]
    public RoleHero role;
    [Header("Attribute Gereral")]
    public float damage;
    public float health;

    [Header("Attribute Defent")]
    public float armor;
    [Header("Attribute probability")]
    public float criticalRate; // tỉ lệ chí mạng
    public float criticalDamageRate; // sát thương chí mạng

    [Header("Speed")]
    public float speed;

    [Header("Duration Attack")]
    public float durationA1;
    public float durationA2;

    [Header("UI")]
    public Sprite iconFace;

    [Header("Power")]
    public Sprite power;

    [Header("HeroPreview")]
    public GameObject HeroPreviewPrefabs;

}
