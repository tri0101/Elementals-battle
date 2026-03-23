using UnityEngine;

public class Effect_Item : MonoBehaviour
{
    [SerializeField] private AbilityEffectType type;

    public void SetType(AbilityEffectType newType)
    {
        type = newType;
    }
    public AbilityEffectType GetAbilityType()
    {
        return type;
    }
}
