using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    private float moveSpeed = 5f;
    private float jumpForce = 7f;
    private float airWalkSpeed = 3f;
    public float stopDistance = 5f;

    float moveX;
    public bool isWalking = false;
    public bool isDashing = false;
    EnemyController ec;
    public bool CanMove
    {

        get => ec.Animator.GetBool("canMove");
        set => ec.Animator.SetBool("canMove", value);
    }
    public bool CanFlip
    {
        get => ec.Animator.GetBool("canFlip");
        set => ec.Animator.SetBool("canFlip", value);
    }
    private void Awake()
    {
        ec = GetComponent<EnemyController>();
    }
    public float CurrentSpeed
    {
        get
        {
            if (CanMove)
            {
                if (isWalking && !ec.CheckingGround.IsOnWall)
                {
                    if (ec.CheckingGround.IsGrounded)
                    {
                        return moveSpeed;
                    }
                    else
                    {
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
    private void Update()
    {
        FollowPlayer();
      


        Flip();
    }
    void FixedUpdate()
    {
       

        ec.Rb.linearVelocity = new Vector2(moveX * moveSpeed, ec.Rb.linearVelocity.y);
        ec.Animator.SetFloat("yVelocity", ec.Rb.linearVelocity.y);

        
    }

    private void LateUpdate()
    {
        AutoFlip();
    }


    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, ec.Player.position);

        if (distance <= stopDistance)
        {
            moveX = 0;
            ec.Animator.SetBool("isWalking", false);
            return;
        }

        moveX = transform.position.x < ec.Player.position.x ? 1 : -1;
        ec.Animator.SetBool("isWalking", true);
    }

    void Flip()
    {
        if (moveX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    void AutoFlip()
    {
        if (!ec.EnemyReceiveDamage.IsAlive) return;
        if (!CanFlip) return;
        if (moveX != 0) return;
        float baseScaleX = Mathf.Abs(transform.localScale.x);
        if (transform.localPosition.x > ec.Player.transform.localPosition.x)
        {
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }
        else
        {

            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}