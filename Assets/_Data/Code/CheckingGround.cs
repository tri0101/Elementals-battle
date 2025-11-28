using System;
using System.Collections;
using UnityEngine;

public class CheckingGround : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    public float rayDistance = 0.05f;
    public float wallCheckDistance = 0.2f;
    public float ceilingDistance = 3.9f;
    CapsuleCollider2D touchingCol;
  
    PlayerController pc;
    EnemyController ec;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    public ContactFilter2D castFilter;

    private Vector2 checkWallDiretion => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [Header("State")]
    [SerializeField] private bool isGrounded = false;
    public bool IsGrounded => isGrounded;
    [SerializeField] private bool isOnWall = false;
    public bool IsOnWall => isOnWall;
    [SerializeField] private bool isOnCeiling = false;
    public bool IsOnCeiling => isOnCeiling;
    private void Awake()
    {
        if(transform.tag == "Player1" || transform.tag == "Player2")
        {
            pc = GetComponent<PlayerController>();
        }
        else
        {
            ec = GetComponent<EnemyController>();
        }
            touchingCol = transform.GetChild(0).Find("ColliderReceive").GetComponent<CapsuleCollider2D>();
    }

    //void Update()
    //{
    //    CheckGround();
    //}
    //private void Awake()
    //{
    //    pc = GetComponent<PlayerController>();
    //}
    //void CheckGround()
    //{

    //    Vector2 origin = (Vector2)transform.position;


    //    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, groundLayer);


    //    Debug.DrawLine(origin, origin + Vector2.down * rayDistance, Color.red);


    //    isGrounded = hit.collider != null;
    //    pc.Animator.SetBool("isGrounded", isGrounded);

    //}
     void FixedUpdate()
    {
        isGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, rayDistance) > 0;
        isOnWall = touchingCol.Cast(checkWallDiretion, castFilter,wallHits, wallCheckDistance) >0;
        isOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
        if(CompareTag("Player1") ||CompareTag("Player2"))
        {
            pc.Animator.SetBool("isGrounded", isGrounded);
            pc.Animator.SetBool("isOnWall", isOnWall);
            pc.Animator.SetBool("isOnCeiling", isOnCeiling);
        }
        else
        {
            ec.Animator.SetBool("isGrounded", isGrounded);
            ec.Animator.SetBool("isOnWall", isOnWall);
            ec.Animator.SetBool("isOnCeiling", isOnCeiling);
        }
       
    }

}
