using UnityEngine;

public class ObjectSpawnPointController : MonoBehaviour
{ 
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    [SerializeField]  ObjectSpawnPointEvent ospe;

    public ObjectSpawnPointEvent ObjectSpawnPointEvent => ospe;
    [SerializeField]  ObjectSpawnPointFly objectSpawnPointFly;
    public ObjectSpawnPointFly ObjectSpawnPointFly => objectSpawnPointFly;
    [SerializeField]  ObjectSpawnPointSO objectSpawnPointSO;
    public ObjectSpawnPointSO ObjectSpawnPointSO => objectSpawnPointSO;

    [SerializeField] private bool canFly;// có thể bay hay ko
  
    private void Awake()
    {
        
        animator = transform.GetChild(0).GetComponent<Animator>();
        ospe = transform.GetChild(0).GetComponent<ObjectSpawnPointEvent>();
        objectSpawnPointFly = GetComponent<ObjectSpawnPointFly>();
        objectSpawnPointSO = GetComponent<ObjectSpawnPointSO>();
        canFly = objectSpawnPointSO.canFly;


    }
}
