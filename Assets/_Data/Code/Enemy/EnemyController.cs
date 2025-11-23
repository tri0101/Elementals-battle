using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    [SerializeField] private PlayerInfo enemyInfo;
    public PlayerInfo EnemyInfo => enemyInfo;
    [SerializeField] private Transform normalT;
    public Transform NormalT => normalT;

    //[SerializeField] private PlayerInfo playerInfo;
    //public PlayerInfo PlayerInfo => playerInfo;
    [SerializeField] private Transform player;
    public Transform Player => player;

    [Header("Staff")]
    EnemyMovement em;
    public EnemyMovement EnemyMovement => em;
    EnemyAttack ea;
    public EnemyAttack EnemyAttack => ea;
    EnemyReceiveDamage erd;
    public EnemyReceiveDamage EnemyReceiveDamage => erd;
    CheckingGround cg;
    public CheckingGround CheckingGround => cg;
  
    PlayerEvent ee;
    public PlayerEvent PlayerEvent => ee;
    [Header("State")]
    private bool hasBeenTransformed = false;
    public bool HasBeenTransformed
    {
        get => animator.GetBool("transformed");
        set => animator.SetBool("transformed", value);
    }
    [SerializeField] private bool isCurrentlyTransforming = false;
    public bool IsCurrentlyTransforming
    {
        get => isCurrentlyTransforming;
        set => isCurrentlyTransforming = value;
    }
    private void Awake()
    {
        normalT = transform.Find("Normal");
        //transformT = transform.Find("Transform");
        animator = normalT.GetComponent<Animator>();
        //transformAnimator = transformT.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        em = GetComponent<EnemyMovement>();
        cg = GetComponent<CheckingGround>();
        ee = transform.GetChild(0).GetComponent<PlayerEvent>();
        erd = transform.GetChild(0).Find("ColliderReceive").GetComponent<EnemyReceiveDamage>();
        ea = GetComponent<EnemyAttack>();
        string targetTag = CompareTag("Player") ? "Enemy" : "Player";
        GameObject enemyObj = GameObject.FindGameObjectWithTag(targetTag);
        player = enemyObj.transform;
    }

}
