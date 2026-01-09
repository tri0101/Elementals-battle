using UnityEngine;
[CreateAssetMenu(menuName = "Player/HeroData")]
public class HeroData : ScriptableObject
{
    public string heroName;

    // 👇 prefab NHÂN VẬT TRONG TRẬN
    public GameObject heroPrefab;
}
