using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private BattleFormation currentFormation;

    private void Awake()
    {
        for (int i = 0; i < currentFormation.slots.Length; i++)
        {
            HeroOfPlayer hero = currentFormation.slots[i];
            if (hero == null) continue;

            Debug.Log($"Spawn hero {hero.heroId} at slot {i}");
        }
    }
}
