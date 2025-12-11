using UnityEngine;

public class ObjectSpawnPointController : MonoBehaviour
{ 
    [SerializeField] private Animator animator;
    public Animator Animator => animator;


    [SerializeField] private ObjectSpawnPointSO objSO;
    public ObjectSpawnPointSO ObjectSpawnPointSO => objSO;
    private void Awake()
    {
        
        animator = transform.GetChild(0).GetComponent<Animator>();

    }
}
