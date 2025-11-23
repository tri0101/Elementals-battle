using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed = 5f;
    private float jumpForce = 7f;
    private float airWalkSpeed = 3f;
    private float dashPower = 20f;
    private float dashDuration = 0.35f;
    private float dashCooldown = 0.5f;
    private Vector2 dashDirection;
    //private bool isBlocking = false;
    public bool IsBlocking
    {
        get => pc.Animator.GetBool("isBlocking");
        set => pc.Animator.SetBool("isBlocking", value);
    }
    float moveX;
    public bool isWalking = false;
    public bool isDashing = false;
    public bool IsDash
    {
        get => pc.Animator.GetBool("isDash");
        set => pc.Animator.SetBool("isDash", value);
    }

    PlayerController pc;
 
    public bool CanMove
    {
       
        get => pc.Animator.GetBool("canMove");
        set => pc.Animator.SetBool("canMove", value);
    }
    public bool CanFlip
    {
        get => pc.Animator.GetBool("canFlip");
        set => pc.Animator.SetBool("canFlip", value);
    }
    public bool CanDash
    {
        get => pc.Animator.GetBool("canDash");
        set => pc.Animator.SetBool("canDash", value);
    }

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    public float CurrentSpeed
    {
        get
        {
            if (CanMove)
            {
                if (isWalking && !pc.CheckingGround.IsOnWall)
                {
                    if (pc.CheckingGround.IsGrounded)
                    {
                        return moveSpeed;
                    }
                    else {
                        return airWalkSpeed; 
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            StartBlock();
        }

        // Nhả S → Hủy Block
        if (Input.GetKeyUp(KeyCode.S))
        {
            StopBlock();
        }

        //Move
        Move();
        // Jump
        if (Input.GetKeyDown(KeyCode.K) && pc.CheckingGround.IsGrounded && CanMove)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.L) )
        {
            StartDash();
        }
        // Flip
        Flip(moveX);
    }
    private void Move()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        // Input
        moveX = Input.GetAxisRaw("Horizontal");
        isWalking = moveX != 0;


        pc.Animator.SetBool("isWalking", isWalking);


    }

    void Jump()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        pc.Rb.linearVelocity = new Vector2(pc.Rb.linearVelocity.x, jumpForce);
        pc.Animator.SetTrigger("isJump");
    }

    void FixedUpdate()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;

        if (IsDash)
        {
            // khi dash, không được động vào velocity
            return;
        }
        if (IsBlocking)
        {
            pc.Rb.linearVelocity = Vector2.zero; // đứng yên khi block
            return;
        }
        pc.Rb.linearVelocity = new Vector2(moveX * CurrentSpeed, pc.Rb.linearVelocity.y);
        pc.Animator.SetFloat("yVelocity", pc.Rb.linearVelocity.y);
    }
    private void LateUpdate()
    {
        AutoFlip();
    }
    void Flip(float moveX)
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        if (!CanFlip) return;
        float baseScaleX = Mathf.Abs(transform.localScale.x);
        if (moveX > 0)
        {
            
            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }
            
        else if (moveX < 0)
        {
            
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }
           

       
    }
    void AutoFlip()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        if (!CanFlip) return;
        if (moveX != 0) return;
        float baseScaleX = Mathf.Abs(transform.localScale.x);
        if (transform.localPosition.x > pc.Enemy.transform.localPosition.x)
        {
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            
            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }
    }
    private void StartDash()
    {
        if ( !CanDash || pc.HasBeenTransformed || IsBlocking) return; 
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        //isDashing = true;
        IsDash = true; 

       
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0)
            x = transform.localScale.x > 0 ? 1 : -1;

        dashDirection = new Vector2(x, y).normalized;

        float start = Time.time;

      
        while (Time.time < start + dashDuration)
        {
            pc.Rb.MovePosition(pc.Rb.position + dashDirection * dashPower * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        //isDashing = false;
        //IsDash = false;
        pc.Rb.linearVelocity = new Vector2(pc.Rb.linearVelocity.x, 0f);
        yield return new WaitForSeconds(0);
    }


    void StartBlock()
    {

        //isBlocking = true;
        IsBlocking = true; 

        
        //CanMove = false;
        //CanFlip = false;

        
        pc.Rb.linearVelocity = Vector2.zero;
    }
    void StopBlock()
    {
        //isBlocking = false;
        IsBlocking = false;

    
        //CanMove = true;
        //CanFlip = true;
    }

}
