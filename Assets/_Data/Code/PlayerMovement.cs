using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed = 5f;
    private float jumpForce = 7f;
    private float airWalkSpeed = 3f;

    float moveX;
    public bool isWalking = false;

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

        //Move
        Move();
        // Jump
        if (Input.GetKeyDown(KeyCode.K) && pc.CheckingGround.IsGrounded && CanMove)
        {
            Jump();
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
        pc.Rb.linearVelocity = new Vector2(moveX * CurrentSpeed, pc.Rb.linearVelocity.y);
    }

    void Flip(float moveX)
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        if (!CanFlip) return;
        if (moveX > 0)
            transform.localScale = new Vector3(5, 5, 5);
        else if (moveX < 0)
            transform.localScale = new Vector3(-5, 5, 5);

        AutoFlip();
    }
    void AutoFlip()
    {
        if (!pc.PlayerReceiveDamage.IsAlive) return;
        if (!CanFlip) return;
        if (moveX != 0) return;
        if (transform.localPosition.x > pc.Enemy.transform.localPosition.x)
        {
            transform.localScale = new Vector3(-5, 5, 5);
        }
        else
        {
            transform.localScale = new Vector3(5, 5, 5);
        }
    }
    //void TranformTo()
    //{
        
    //    if (pc.HasBeenTransformed || pc.IsCurrentlyTransforming)
    //    {
    //        return;
    //    }

    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
            
    //        pc.IsCurrentlyTransforming = true; 

    //        pc.Animator.SetTrigger("transform");

            
    //        pc.HasBeenTransformed = true; 
    //    }
    //}
    void BackToHuman()
    {

    }
}
