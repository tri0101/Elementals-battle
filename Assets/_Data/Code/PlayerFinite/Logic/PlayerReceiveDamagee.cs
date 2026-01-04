using UnityEngine;

public class PlayerReceiveDamagee : MonoBehaviour, IObserver
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
    [SerializeField] private bool isDead = false;
    public bool IsStopAnim
    {
        get => isStopAnim; set => isStopAnim = value;
    }
   
    public bool IsDead
    {
        get => isDead; set => isDead = value;
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
        maxMana = maxHealth * 2;

        mana = 1000000f;
        //mana = 0f;
        health = maxHealth;
        playerControl.RefreshObservers();

    }


    public void ReceiveDamage(float damage)
    {
        

        if (isDead) return;
        if(playerControl.CurrentStringState == "Block" || playerControl.CurrentStringState == "T_Block")
        {
            mana += damage ;
            playerControl.RefreshObservers();
            return;
        }
        
        health -= damage;

        mana += damage * 1.5f;

        //if(mana >= 1000f)
        //{
        //    mana = 1000f;
        //}
        isHit = true;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
          

        }
        playerControl.RefreshObservers();
    }

    public void AddManaWhenAttack(float damage)
    {
        mana += damage * 2;
        if(mana >= 1000)
        {
            mana = 1000f;
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
    public void OnNotify()
    {

    }
}
