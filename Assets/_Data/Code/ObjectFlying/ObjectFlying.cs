using NUnit.Framework.Internal;
using UnityEngine;

public class ObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    [Header("Speed")]
    public float speed = 5f;

    [Header("Direction")]
    private bool isFlyingRight = true; 

    [SerializeField] private bool canFly = true; 
    public bool CanFly
    {
        get => canFly; set => canFly = value;
    }


    private Vector3 moveDir;

    private void Start()
    {
        objController = GetComponent<ObjectFlyingController>();
        SetDirection(isFlyingRight);
        
    }

    private void Update()
    {
        if (canFly)
        {
            Flying();

        }
      
    }

    private void Flying()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }
    public void SetDirection(bool flyRight)
    {
        isFlyingRight = flyRight;

        if (isFlyingRight)
        {
            moveDir = Vector3.right;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            moveDir = Vector3.left;

            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    
 


}
