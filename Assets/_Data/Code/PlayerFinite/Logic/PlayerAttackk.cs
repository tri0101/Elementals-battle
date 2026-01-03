using System.Threading;
using UnityEngine;

public class PlayerAttackk : MonoBehaviour, IObserver
{
    PlayerControl playerControl;
    [SerializeField] private int countAttack = 0;
    [SerializeField] Transform colliderAttack;
    public int CountAttack
    {
        get => countAttack;
        set => countAttack = value;
    }
    
    public PlayerControl PlayerControlPlayer => playerControl;
    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
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
            playerControl.PlayerReceiveDamagee.AddManaWhenAttack(attack.AttackDamage);
        }
        
    }
    public void OnNotify() { }
}
