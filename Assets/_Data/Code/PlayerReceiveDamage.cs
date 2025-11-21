using UnityEngine;

public class PlayerReceiveDamage : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] private float health;
    [SerializeField] private bool isHit;
    [SerializeField] private bool isAlive = true;

    [SerializeField] private CapsuleCollider2D capsuleCollider;
    public bool IsAlive
    {
        get => pc.Animator.GetBool("isAlive");
        set => pc.Animator.SetBool("isAlive", value);
    }
    public bool IsHit
    {
        get => pc.Animator.GetBool("isHit");
        set => pc.Animator.SetBool("isHit", value);
    }
    private void Awake()
    {
        pc = transform.parent.parent.GetComponent<PlayerController>();
        health = pc.PlayerInfo.health;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    public void ReceiveDamage(float damage)
    {
        if (!IsAlive) return;
        health -= damage;
        IsHit = true;
        if (health <= 0)
        {
            health = 0;
            IsAlive = false;
           
        }
        
    }
    public void CallKnockBack(Vector3 knockPosition)
    {
        pc.PlayerEvent.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, 0.1f);
    }
    
}
