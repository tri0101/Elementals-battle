using System.Threading;
using UnityEngine;

public class HeroAttackk : MonoBehaviour
{
    HeroControl heroControl;
    [SerializeField] private int countAttack = 0;
    [SerializeField] Transform colliderAttack;
    public int CountAttack
    {
        get => countAttack;
        set => countAttack = value;
    }
    
    public HeroControl HeroControl => heroControl;
    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
        colliderAttack = transform.GetChild(0).GetChild(0);
        
    }
    
}
