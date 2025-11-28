using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;


    [SerializeField] private Transform normalT;
    public Transform NormalT => normalT;

    [SerializeField] private PlayerInfo playerInfo;
    public PlayerInfo PlayerInfo => playerInfo;
    [SerializeField] private Transform enemy;
    public Transform Enemy => enemy;
    
    [Header("Staff")]
    PlayerMovement pm;
    public PlayerMovement PlayerMovement => pm;
    CheckingGround cg;
    public CheckingGround CheckingGround => cg;
    PlayerAttack pa;
    public PlayerAttack PlayerAttack => pa;
    PlayerSkill ps;
    public PlayerSkill PlayerSKill => ps;
    PlayerTransform pt;
    public PlayerTransform PlayerTransform => pt;
    PlayerReceiveDamage prd;
    public PlayerReceiveDamage PlayerReceiveDamage => prd;
    PlayerEvent pe;
    public PlayerEvent PlayerEvent => pe;
    [Header("KeyBiding")]
    [SerializeField] KeyBinding keyBiding;
    public KeyBinding KeyBiding => keyBiding;
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
        pm = GetComponent<PlayerMovement>();
        cg = GetComponent<CheckingGround>();
        ps = gameObject.GetComponent<PlayerSkill>();
        pt = GetComponent<PlayerTransform>();
        pe = transform.GetChild(0).GetComponent<PlayerEvent>();
        prd = transform.GetChild(0).Find("ColliderReceive").GetComponent<PlayerReceiveDamage>();
        string targetTag = CompareTag("Player1") ? "Player2" : "Player1";
        GameObject enemyObj = GameObject.FindGameObjectWithTag(targetTag);
        enemy = enemyObj.transform;
        
    }
    
}
