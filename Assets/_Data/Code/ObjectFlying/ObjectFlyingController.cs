using UnityEngine;

public class ObjectFlyingController : MonoBehaviour
{
    [SerializeField] private ObjectFlying objF;
    public ObjectFlying ObjectFlying => objF;

    [SerializeField] private CheckingObjectFlying cObjF;
    public CheckingObjectFlying CheckingObjectFlying => cObjF;
    [SerializeField] private ObjectFlyingAttack objAttack;
    public ObjectFlyingAttack ObjectFlyingAttack => objAttack;

    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    private void Awake()
    {
        objF = GetComponent<ObjectFlying>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        cObjF = GetComponent<CheckingObjectFlying>();
        objAttack = GetComponent<ObjectFlyingAttack>();
    }
}
