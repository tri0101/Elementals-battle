using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerCheckingGround : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    public float rayDistance = 0.1f;
    public float rayUnderDistance = 0.5f;
    public float wallCheckDistance = 0.2f;
    public float checkXJump = 3f;
    public float ceilingDistance = 3.9f;
    CapsuleCollider2D touchingCol;
    public CapsuleCollider2D TouchingCol => touchingCol;

    PlayerControl playerControl;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    public ContactFilter2D castFilter;
    public LayerMask playerLayer;
    RaycastHit2D[] playerHits = new RaycastHit2D[5];

    private Vector2 checkWallDiretion => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [Header("State")]
    [SerializeField] private bool isGrounded = false;
    public bool IsGrounded => isGrounded;
    [SerializeField] private bool isOnWall = false;
    public bool IsOnWall => isOnWall;
    [SerializeField] private bool isOnCeiling = false;
    public bool IsOnCeiling => isOnCeiling;
    [SerializeField] private bool isOnPlayer = false;
    public bool IsOnPlayer => isOnPlayer;
    [SerializeField] private bool isUnderPlayer = false;
    public bool IsUnderPlayer => isUnderPlayer;
    [SerializeField] private bool isXJumpPlayer = false;
    public bool IsXJumpPlayer => isXJumpPlayer;
    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();

        touchingCol = transform.GetChild(0).Find("ColliderReceive").GetComponent<CapsuleCollider2D>();
    }
    void FixedUpdate()
    {
        //isOnWall = touchingCol.Cast(checkWallDiretion, castFilter, wallHits, wallCheckDistance) > 0;
        bool hitRight = touchingCol.Cast(
           Vector2.right, castFilter, wallHits, wallCheckDistance) > 0;

        bool hitLeft = touchingCol.Cast(
            Vector2.left, castFilter, wallHits, wallCheckDistance) > 0;

        isOnWall = hitRight || hitLeft;
        isGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, rayDistance) > 0;

        isOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
        isOnPlayer = touchingCol.Cast(Vector2.down, new ContactFilter2D { layerMask = playerLayer, useLayerMask = true }, playerHits, rayDistance) > 0;
        isUnderPlayer = touchingCol.Cast(Vector2.up, new ContactFilter2D { layerMask = playerLayer, useLayerMask = true }, playerHits, rayUnderDistance) > 0;
        isXJumpPlayer = touchingCol.Cast(checkWallDiretion, new ContactFilter2D { layerMask = playerLayer, useLayerMask = true }, playerHits, checkXJump) > 0;
       



    }
}
