using UnityEngine;

[CreateAssetMenu(menuName ="hero/heroInfo")]
public class HeroInfo : ScriptableObject
{

    [Header("ID")]
    public int ID;

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


  
}
