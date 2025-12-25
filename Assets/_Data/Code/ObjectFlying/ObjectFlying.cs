using NUnit.Framework.Internal;
using UnityEngine;
using System.Collections;
public class ObjectFlying : MonoBehaviour
{
    ObjectFlyingController objController;
    [SerializeField]  private float despawnTimer;
    [SerializeField ] private float timeToDespawn;
    [SerializeField] private bool despawnTriggered = false;
    [SerializeField] private bool hasbeenToPool = false;
    public bool HasBeenToPool
    {
        get => hasbeenToPool;
        set => hasbeenToPool = value;
    }
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

    private void Awake()
    {
        objController = GetComponent<ObjectFlyingController>();
        speed = objController.ObjectFlyingSO.flySpeed;
        timeToDespawn = objController.ObjectFlyingSO.timeToDespawn;
        //SetDirection();
        //if (hasbeenToPool) return;
        //StartCoroutine(DespawnAfterTime());
        //hasbeenToPool = true;
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
                    if (hasbeenToPool) return;
                    despawnTriggered = true;
                    hasbeenToPool = true;
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
            
            moveDir = objController.ObjectFlyingSO.directionFly;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            Vector3 temp = objController.ObjectFlyingSO.directionFly;
            moveDir = new Vector3(-temp.x, temp.y, temp.z);

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
        hasbeenToPool = false;
    }



}
