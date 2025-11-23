using UnityEngine;

public class EnemyReceiveDamage : MonoBehaviour
{
    EnemyController ec;
    [SerializeField] private float health;
    [SerializeField] private bool isHit;
    [SerializeField] private bool isAlive = true;

    [SerializeField] private CapsuleCollider2D capsuleCollider;
    public bool IsAlive
    {
        get => ec.Animator.GetBool("isAlive");
        set => ec.Animator.SetBool("isAlive", value);
    }
    public bool IsHit
    {
        get => ec.Animator.GetBool("isHit");
        set => ec.Animator.SetBool("isHit", value);
    }
    private void Awake()
    {
        ec = transform.parent.parent.GetComponent<EnemyController>();
        health = ec.EnemyInfo.health;
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
        ec.PlayerEvent.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, 0.1f);
    }
}
