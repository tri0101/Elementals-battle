using UnityEngine;
using System.Collections;
public class PlayerReceiveDamage : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] private float health;
    [SerializeField] private bool isHit;
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isFinalAttack = false;
    [SerializeField] private float durationFinalAttack;
    public bool IsFinalAttack
    {
        get => isFinalAttack; set => isFinalAttack = value;
    } 
    public float DurationFinalAttack
    {
        get => durationFinalAttack; set => durationFinalAttack = value;
    }
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
    public bool IsImmortal
    {
        get => pc.Animator.GetBool("isImmortal");
        set => pc.Animator.SetBool("isImmortal", value);
    }
    private void Awake()
    {
        pc = transform.parent.parent.GetComponent<PlayerController>();
        health = pc.PlayerInfo.health;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    public void ReceiveDamage(float damage)
    {
        if (!IsAlive || IsImmortal) return;
        health -= damage;
        IsHit = true;
        if (health <= 0)
        {
            health = 0;
            IsAlive = false;
           
        }
        
    }
    public void CallKnockBack(Vector3 knockPosition, float duration)
    {
        if (IsImmortal) return;
        pc.PlayerEvent.CallSlideToPosition(transform.parent.parent.localPosition, knockPosition, duration);
    }
    public void CallKnockBackBySpeed(Vector3 knockPosition, float speed)
    {
        if (IsImmortal) return;
        pc.PlayerEvent.CallSlideToPositionBySpeed( knockPosition, speed);
    }
    public void CallApplyKnockBack(Vector2 attackerPos)
    {
        
        float force = 50f;
        float slowDuration = 0.2f;
        StartCoroutine(KnockbackRoutine(attackerPos, force, slowDuration));
    }


    private IEnumerator KnockbackRoutine(Vector2 attackerPos, float force, float slowDuration)
    {
        // 1. Hướng knockback
        Vector2 dir = ((Vector2)transform.parent.parent.position - attackerPos).normalized;

        // 2. Apply force ngay lập tức
        pc.Rb.linearVelocity = dir * force;

        // 3. Hitstop
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.06f);
        Time.timeScale = 1f;

        // 4. Slow motion
        Time.timeScale = 0.25f;
        float timer = 0f;
        while (timer < slowDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 5. Reset time
        Time.timeScale = 1f;
    }
    public void CallIsFinal(float duration)
    {
        isFinalAttack = true;
        DurationFinalAttack = duration;
    }
    public void ResetDurationAttack()
    {
        isFinalAttack = false;
        durationFinalAttack = 0f;
    }

}
