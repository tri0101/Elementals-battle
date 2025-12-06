using NUnit.Framework.Internal;
using UnityEngine;

public class ObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    public ObjectFlyingController ObjController => objController;
    [Header("Speed")]
    public float speed;

    [Header("Direction")]
    private bool isFlyingRight => transform.localScale.x > 0 ? true : false;

    [SerializeField] private bool canFly = true; 
    public bool CanFly
    {
        get => canFly; set => canFly = value;
    }


    private Vector3 moveDir;

    private void Start()
    {
        objController = GetComponent<ObjectFlyingController>();
        speed = objController.ObjectFlyingSO.flySpeed;
        SetDirection();
        
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
    public void SetDirection()
    {
       

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
