using UnityEngine;

public class PlayerReceiveDamagee : MonoBehaviour
{

    [SerializeField] PlayerControl playerControl;
    public PlayerControl PlayerControl => playerControl;


    [Header("Attribute")]
    [SerializeField] private float health;


    [Header("Flag")]
    [SerializeField] private bool isHit;
    public bool IsHit
    {
        get => isHit;
        set => isHit = value;
    }
    private void Awake()
    {
        playerControl = transform.parent.parent.GetComponent<PlayerControl>();
        health = playerControl.PlayerInfo.health;


    }

    public void ReceiveDamage(float damage)
    {
        

        
        health -= damage;


        isHit = true;
        if (health <= 0)
        {
            health = 0;
          

        }

    }
   
}
