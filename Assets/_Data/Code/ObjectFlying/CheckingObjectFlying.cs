using UnityEngine;

public class CheckingObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    BoxCollider2D touchingCol;
    public BoxCollider2D TouchingCol => touchingCol;

    [SerializeField] private bool isOnWall = false;
    public bool IsOnWall => isOnWall;


    public float wallCheckDistance = 0.2f;
    public ContactFilter2D castFilter;
  
    private Vector2 checkWallDiretion => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    private void Start()
    {
        touchingCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
        objController = GetComponent<ObjectFlyingController>();
    }
    void FixedUpdate()
    {
        isOnWall = touchingCol.Cast(checkWallDiretion, castFilter, wallHits, wallCheckDistance) > 0;


    }

}
