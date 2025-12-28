using UnityEngine;

public class PlayerReceiveDamagee : MonoBehaviour
{

    [SerializeField] PlayerControl playerControl;
    public PlayerControl PlayerControl => playerControl;


    [Header("Attribute")]
    [SerializeField] private float health;
    
    [SerializeField] private float durationFinalAttack;

    public float DurationFinalAttack
    {
        get => durationFinalAttack; set => durationFinalAttack = value;
    }

    [Header("Flag")]
    [SerializeField] private bool isHit;
    [SerializeField] private bool isStopAnim = false;
    public bool IsStopAnim
    {
        get => isStopAnim; set => isStopAnim = value;
    }
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
    public void CallStopAnim(float duration)
    {
        isStopAnim = true;
        DurationFinalAttack = duration;
    }
    public void CallKnockBack(Vector3 knockPosition, float duration)
    {
       
        
        playerControl.PlayerEventt.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, duration);
    }
}
