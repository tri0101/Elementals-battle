using UnityEngine;

public class CheckingObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    RaycastHit2D[] playerHits = new RaycastHit2D[5];
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    BoxCollider2D touchingCol;
    public BoxCollider2D TouchingCol => touchingCol;

    [SerializeField] private bool isTouchPlayer = false;
    public bool IsTouchPlayer => isTouchPlayer;
    [SerializeField] private bool isTouchGround = false;
    public bool IsTouchGround => isTouchGround;


    public float playerCheckDistance = 0.2f;
    public float rayGroundDistance = 0.1f;
    public ContactFilter2D castFilter;
  
    private Vector2 checkWallDiretion => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    private void Awake()
    {
        touchingCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
        objController = GetComponent<ObjectFlyingController>();
        if (transform.tag == "RangedAttackPlayer1")
        {
            castFilter.layerMask = LayerMask.GetMask("Player2", "Ground");
            //transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("RangedPlayer1");
        }
        else if (transform.tag == "RangedAttackPlayer2")
        {
            castFilter.layerMask = LayerMask.GetMask("Player1", "Ground");
            //transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("RangedPlayer2");
        }
       
    }
    void FixedUpdate()
    {
        isTouchPlayer = touchingCol.Cast(checkWallDiretion, castFilter, playerHits, playerCheckDistance) > 0;
        isTouchGround = touchingCol.Cast(Vector2.down, castFilter, groundHits, rayGroundDistance) > 0;

    }


    private void OnDisable()
    {
        isTouchPlayer = false;
    }
}
