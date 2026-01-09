using UnityEngine;

public class BattleSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        HeroData data = GameManager.Instance.selectedHero;
        Instantiate(data.heroPrefab, spawnPoint.position, Quaternion.identity);
    }
}
