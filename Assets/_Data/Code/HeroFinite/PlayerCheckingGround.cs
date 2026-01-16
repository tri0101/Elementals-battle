using NUnit.Framework.Internal;
using UnityEngine;

public class HheroCheckingGround : MonoBehaviour
{ 
//{
//    [Header("Ground Check Settings")]
//    public LayerMask groundLayer;
//    public float rayDistance = 0.1f;
//    public float rayUnderDistance = 2.5f;
//    public float wallCheckDistance = 0.2f;
//    public float checkXJump = 0.5f;
//    public float ceilingDistance = 3.9f;
//    CapsuleCollider2D touchingCol;
//    public CapsuleCollider2D TouchingCol => touchingCol;

//    heroControl heroControl;

//    RaycastHit2D[] groundHits = new RaycastHit2D[5];
//    RaycastHit2D[] wallHits = new RaycastHit2D[5];
//    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
//    public ContactFilter2D castFilter;
//    public LayerMask heroLayer;
//    RaycastHit2D[] heroHits = new RaycastHit2D[5];

//    private Vector2 checkWallDiretion => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

//    [Header("State")]
//    [SerializeField] private bool isGrounded = false;
//    public bool IsGrounded => isGrounded;
//    [SerializeField] private bool isOnWall = false;
//    public bool IsOnWall => isOnWall;
//    [SerializeField] private bool isWallLeft = false;
//    public bool IsWallLeft => isWallLeft;
//    [SerializeField] private bool isWallRight = false;
//    public bool IsWallRight => isWallRight;
//    [SerializeField] private bool isOnCeiling = false;
//    public bool IsOnCeiling => isOnCeiling;
//    [SerializeField] private bool isOnhero = false;
//    public bool IsOnhero => isOnhero;
//    [SerializeField] private bool isUnderhero = false;
//    public bool IsUnderhero => isUnderhero;
//    [SerializeField] private bool isXJumphero = false;
//    public bool IsXJumphero => isXJumphero;
//    private void Awake()
//    {
//        heroControl = GetComponent<heroControl>();

//        touchingCol = transform.GetChild(0).Find("ColliderReceive").GetComponent<CapsuleCollider2D>();
//        if(transform.tag == "hero1")
//        {
//            heroLayer = LayerMask.GetMask("hero2");
//        }
//        else
//        {
//            heroLayer = LayerMask.GetMask("hero1");
//        }
//    }
//    void FixedUpdate()
//    {
//        //isOnWall = touchingCol.Cast(checkWallDiretion, castFilter, wallHits, wallCheckDistance) > 0;
//        isWallRight = touchingCol.Cast(
//           Vector2.right, castFilter, wallHits, wallCheckDistance) > 0;

//        isWallLeft = touchingCol.Cast(
//            Vector2.left, castFilter, wallHits, wallCheckDistance) > 0;

//        isOnWall = isWallRight || isWallLeft;
//        isGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, rayDistance) > 0;

//        isOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
//        isOnhero = touchingCol.Cast(Vector2.down, new ContactFilter2D { layerMask = heroLayer, useLayerMask = true }, heroHits, rayDistance) > 0;
//        //isUnderhero = touchingCol.Cast(Vector2.up, new ContactFilter2D { layerMask = heroLayer, useLayerMask = true }, heroHits, rayUnderDistance) > 0;
//        isXJumphero = touchingCol.Cast(checkWallDiretion, new ContactFilter2D { layerMask = heroLayer, useLayerMask = true }, heroHits, checkXJump) > 0;


//        Vector2 origin = touchingCol.bounds.center;

//        RaycastHit2D hit = Physics2D.Raycast(
//            origin,
//            Vector2.up,
//            rayUnderDistance,
//            heroLayer
//        );

//        isUnderhero = hit.collider != null;

    
}
