using UnityEngine;
using System.Collections;
public class PlayerReceiveDamage : MonoBehaviour
{
    PlayerController pc;
    public PlayerController Pc => pc;
    [SerializeField] private float health;
    [SerializeField] private bool isHit;
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isFinalAttack = false;
    [SerializeField] private bool isStopAnim = false;
    [SerializeField] private float durationFinalAttack;



    public bool IsFinalAttack
    {
        get => isFinalAttack; set => isFinalAttack = value;
    } 
    public bool IsStopAnim
    {
        get => isStopAnim; set => isStopAnim = value;
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
        
 
    }
    private void Start()
    {
        capsuleCollider = pc.CheckingGround.TouchingCol;
    }
    public void ReceiveDamage(float damage)
    {
        if (!IsAlive || IsImmortal || pc.PlayerMovement.IsBlocking) return;
        pc.Animator.speed = 1f;
        pc.Animator.SetTrigger("Hit");
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
        if (pc.PlayerMovement.IsBlocking) {
            knockPosition.x = -0.2f;
            duration = 0.01f;
        }
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
    public void CallStopAnim(float duration)
    {
        isStopAnim = true;
        DurationFinalAttack = duration;
    }
    public void ResetDurationAttack()
    {
       
        durationFinalAttack = 0f;
        if (isFinalAttack)
        {
            StartCoroutine(FinalAttackImmortalCoroutine());
        }
        
    }
    private IEnumerator FinalAttackImmortalCoroutine()
    {
        IsImmortal = true; // ✅ Bật bất tử
        yield return new WaitForSeconds(5f);   // ✅ 0.15 giây
        IsImmortal = false; // ✅ Tắt bất tử
        isFinalAttack = false; // ✅ Reset Final Attack
    }
    public void CallIsFinal()
    {
        isFinalAttack = true;
    }
    void FixedUpdate()
    {
        if (pc.CheckingGround.IsOnPlayer && pc.Rb.linearVelocity.y < 0)
        {
            // Tạo hiệu ứng trượt xuống người khác
            Vector2 vel = pc.Rb.linearVelocity;
            vel.y = -50f; // tốc độ rơi trượt
            pc.Rb.linearVelocity = vel;
        }
        
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        // Chỉ xử lý khi va chạm với collider tên "ColliderReceive"
        if (col.collider.name != "ColliderReceive")
        {
            
            return;
        }

        // Lấy tag của đối tượng cha (Rigidbody root)
        if (pc.CheckingGround.IsOnPlayer)
        {
           
            Physics2D.IgnoreCollision(capsuleCollider, col.collider, true);
        }
        else if(pc.CheckingGround.IsXJumpPlayer && !pc.CheckingGround.IsGrounded)
        {
            Physics2D.IgnoreCollision(capsuleCollider, col.collider, true);
        }

    }

    private void OnCollisionExit2D(Collision2D col)
    {
        // Chỉ xử lý khi collider là "ColliderReceive"
        if (col.collider.name != "ColliderReceive")
        {
            
            return;
        }

        if (pc.CheckingGround.IsGrounded) {
            Physics2D.IgnoreCollision(capsuleCollider, col.collider, false);

        }


    }


}
