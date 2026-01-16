using System.Threading;
using UnityEngine;

public class HeroAttackk : MonoBehaviour, IObserver
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
    private void Start()
    {
        RegisterAttackObservers();
    }

    void RegisterAttackObservers()
    {
        Attack[] attacks = colliderAttack.GetComponentsInChildren<Attack>(true);

        foreach (var attack in attacks)
        {
            if (!attack.transform.name.Contains("Skill"))
            {
                attack.AddObserver(this);
            }
            
        }
    }
    public void OnNotify(object data)
    {
        if(data is Attack attack)
        {
            heroControl.HeroReceiveDamagee.AddManaWhenAttack(attack.AttackDamage);
        }
        
    }
    public void OnNotify() { }
}
