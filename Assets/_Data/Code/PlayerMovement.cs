using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    private float jumpForce;
    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }
    private float airWalkSpeed;
    public float AirWalkSpeed
    {
        get => airWalkSpeed;
        set => airWalkSpeed = value;
    }
    private float dashPower;
    private float dashDuration;
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
    public bool CanJump
    {
        get => pc.Animator.GetBool("canJump");
        set => pc.Animator.SetBool("canJump", value);
    }

    private bool isLockHorizontal = false;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        moveSpeed = pc.PlayerInfo.moveSpeed;
        jumpForce = pc.PlayerInfo.jumpForce;
        airWalkSpeed = pc.PlayerInfo.airWalkSpeed;
        dashPower = pc.PlayerInfo.dashPower;
        dashDuration = pc.PlayerInfo.dashDuration;
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
        if (Input.GetKey(pc.KeyBiding.blockKey) && pc.CheckingGround.IsGrounded)
        {
            //Flip khi block
            StartBlock();
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

        // Nhả S → Hủy Block
        if (Input.GetKeyUp(pc.KeyBiding.blockKey))
        {
            StopBlock();
        }

        //Move
        Move();
   

        // Jump
        if (Input.GetKeyDown(pc.KeyBiding.jumpKey) && pc.CheckingGround.IsGrounded && CanMove && CanJump)
        {
            if (pc.CheckingGround.IsXJumpPlayer)
            {
                CanMove = false;
                StartCoroutine(ResetCanMove());
            }
            Jump();
        }
        if (Input.GetKeyDown(pc.KeyBiding.dashKey) )
        {
            StartDash();
        }
        // Flip
        Flip(moveX);
    }
    private IEnumerator ResetCanMove()
    {
        yield return new WaitForSeconds(0.35f);
        CanMove = true;
    }
    private void Move()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        // Input
        moveX = 0;   

        if (Input.GetKey(pc.KeyBiding.leftMove))
            moveX = -1;
        else if (Input.GetKey(pc.KeyBiding.rightMove))
            moveX = 1;
        else
            moveX = 0;


        isWalking = moveX != 0;


        pc.Animator.SetBool("isWalking", isWalking);


    }

    void Jump()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        //if (isLockHorizontal)
        //{
        //    pc.Rb.linearVelocity = new Vector2(0, jumpForce);
        //}
        //else
        //{
        //    pc.Rb.linearVelocity = new Vector2(pc.Rb.linearVelocity.x, jumpForce);
        //}
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


        if (pc.CheckingGround.IsOnPlayer && pc.Rb.linearVelocity.y < 0)
        {
            // Tạo hiệu ứng trượt xuống người khác
            Vector2 vel = pc.Rb.linearVelocity;
            vel.y = -50f; // tốc độ rơi trượt
            pc.Rb.linearVelocity = vel;
        }
        if (pc.CheckingGround.IsUnderPlayer && pc.Rb.linearVelocity.y < 0)
        {
            Vector3 pos = transform.position;
            if(transform.localScale.x > 0)
            {
                pos.x -= 1.5f;
            }
            else
            {
                pos.x += 1.5f;
            }
            transform.position = pos;
        }
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

        float x = 0f;
        float y = 0f;
        if (transform.tag == "Player1")
        {
            if (Input.GetKey(KeyCode.A)) x = -1;
            if (Input.GetKey(KeyCode.D)) x = 1;
        }
       
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow)) x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) x = 1;
        }


        if (x == 0 && y == 0)
            x = transform.localScale.x > 0 ? 1 : -1;

        dashDirection = new Vector2(x, y).normalized;

        float start = Time.time;

      
        while (Time.time < start + dashDuration)
        {
            pc.Rb.MovePosition(pc.Rb.position + dashDirection * dashPower * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

  
        pc.Rb.linearVelocity = new Vector2(pc.Rb.linearVelocity.x, 0f);
        yield return new WaitForSeconds(0);
    }


    void StartBlock()
    {

       
        IsBlocking = true; 

   
        
        pc.Rb.linearVelocity = Vector2.zero;
    }
    void StopBlock()
    {
 
        IsBlocking = false;

    }
    public void MinusProperty(float percent)
    {
        moveSpeed -= moveSpeed * percent; 
        jumpForce -= jumpForce * percent;
        airWalkSpeed -= airWalkSpeed * percent;
        dashPower -= dashPower * percent;
    }
    public void PlusProperty(float percent)
    {
        moveSpeed += moveSpeed * percent;
        jumpForce += jumpForce * percent;
        airWalkSpeed += airWalkSpeed * percent;
        dashPower += dashPower * percent;
    }
}
