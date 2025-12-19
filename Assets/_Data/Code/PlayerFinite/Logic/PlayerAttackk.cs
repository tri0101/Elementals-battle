using System.Threading;
using UnityEngine;

public class PlayerAttackk : MonoBehaviour
{
    PlayerControl playerControl;
    [SerializeField] private int countAttack = 0;
    public int CountAttack
    {
        get => countAttack;
        set => countAttack = value;
    }
    
    public PlayerControl PlayerControlPlayer => playerControl;
    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }
}
