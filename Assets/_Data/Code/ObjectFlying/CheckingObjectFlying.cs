using UnityEngine;

public class CheckingObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    RaycastHit2D[] heroHits = new RaycastHit2D[5];
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    BoxCollider2D touchingCol;
    public BoxCollider2D TouchingCol => touchingCol;

    [SerializeField] private bool isTouchhero = false;
    public bool IsTouchhero => isTouchhero;
    [SerializeField] private bool isTouchGround = false;
    public bool IsTouchGround => isTouchGround;


    public float heroCheckDistance = 0.2f;
    public float rayGroundDistance = 0.1f;
    public ContactFilter2D castFilter;
    public ContactFilter2D heroFilter;
    
    private Vector2 checkWallDiretion => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    private void Awake()
    {
        touchingCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
        objController = GetComponent<ObjectFlyingController>();
        if (transform.tag == "RangedAttackhero1")
        {
            castFilter.layerMask = LayerMask.GetMask("Ground");
            heroFilter.layerMask = LayerMask.GetMask("hero2");
            //transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Rangedhero1");
        }
        else if (transform.tag == "RangedAttackhero2")
        {
            castFilter.layerMask = LayerMask.GetMask("Ground");
            heroFilter.layerMask = LayerMask.GetMask("hero1");
            //transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Rangedhero2");
        }
       
    }
    void FixedUpdate()
    {
        isTouchhero = touchingCol.Cast(checkWallDiretion, heroFilter, heroHits, heroCheckDistance) > 0;
        isTouchGround = touchingCol.Cast(Vector2.down, castFilter, groundHits, rayGroundDistance) > 0;

    }


    private void OnDisable()
    {
        isTouchhero = false;
        isTouchGround = false;
    }
}
