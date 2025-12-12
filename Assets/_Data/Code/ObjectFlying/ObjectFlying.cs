using NUnit.Framework.Internal;
using UnityEngine;
using System.Collections;
public class ObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    private float despawnTimer;
    private float timeToDespawn;
    private bool despawnTriggered = false;
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
        timeToDespawn = objController.ObjectFlyingSO.timeToDespawn;
        SetDirection();
        StartCoroutine(DespawnAfterTime());
    }
    private void OnEnable()
    {
        SetDirection();
    }

    private void Update()
    {
        if (canFly)
        {
            Flying();
            if (!despawnTriggered)
            {
                despawnTimer += Time.deltaTime;

                if (despawnTimer >= timeToDespawn)
                {
                    despawnTriggered = true;
                    ObjectFlyingSpawnPoint.instance.AddToPool(gameObject);
                }
            }
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
    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(objController.ObjectFlyingSO.timeToDespawn);

        ObjectFlyingSpawnPoint.instance.AddToPool(gameObject);
    }


    private void OnDisable()
    {
        canFly = true;
        despawnTimer = 0;
        despawnTriggered = false;
    }



}
