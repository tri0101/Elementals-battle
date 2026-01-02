using UnityEngine;

public class PlayerReceiveDamagee : MonoBehaviour
{

    [SerializeField] PlayerControl playerControl;
    public PlayerControl PlayerControl => playerControl;


    [Header("Attribute")]
    [SerializeField] private float health;
    public float Health
    {
        get => health; set => health = value;
    }
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth; set => maxHealth = value;
    }
    
    [SerializeField] private float mana;
    public float Mana
    {
        get => mana; set => mana = value;
    }
    [SerializeField] private float maxMana;
    public float MaxMana
    {
        get => maxMana; set => maxMana = value;
    }
    
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

       
        maxHealth = playerControl.PlayerInfo.health;
        maxMana = 15f;
        mana = 0f;
        health = maxHealth;
        playerControl.RefreshObservers();

    }


    public void ReceiveDamage(float damage)
    {
        

        
        health -= damage;

        mana += 2;
       
        isHit = true;
        if (health <= 0)
        {
            health = 0;
          

        }
        playerControl.RefreshObservers();
    }
    public void CallStopAnim(float duration)
    {
        //isStopAnim = true;
        //stop anim trong vong duration
        DurationFinalAttack = duration;
    }
    public void CallKnockBack(Vector3 knockPosition, float duration)
    {
       
        
        playerControl.PlayerEventt.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, duration);
    }
    public float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }
    public float GetManaPercent()
    {
        return (float)mana / maxMana;
    }
}
