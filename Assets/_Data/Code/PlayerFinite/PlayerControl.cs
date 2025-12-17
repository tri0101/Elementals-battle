using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Attribute")]
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    [SerializeField] private Transform normalT;
    public Transform NormalT => normalT;

    public float MoveX;

    private bool isJumpPressed;

    public bool IsJumpPressed
    {
        get => isJumpPressed;
        set => isJumpPressed = value;
    }
    private string currentStringState;

    [Header("Other Script")]
    PlayerCheckingGround playerCheckingGround;

    public PlayerCheckingGround PlayerCheckingGround => playerCheckingGround;
    PlayerStateManager playerStateManager;
    public PlayerStateManager PlayerStateManager => playerStateManager;

    PlayerRun playerRun;
    public PlayerRun PlayerRun => playerRun;
    PlayerJump playerJump;
    public PlayerJump PlayerJump => playerJump;

    [Header("ScriptableObject")]
    [SerializeField] private PlayerInfo playerInfo;
    public PlayerInfo PlayerInfo => playerInfo;
    [Header("KeyBiding")]
    [SerializeField] KeyBinding keyBiding;
    public KeyBinding KeyBiding => keyBiding;
    private void Awake()
    {
        normalT = transform.Find("Normal");
        animator = normalT.GetComponent<Animator>();
        spriteRenderer = normalT.GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        playerCheckingGround = GetComponent<PlayerCheckingGround>();
        playerStateManager = GetComponent<PlayerStateManager>();
        playerRun = GetComponent<PlayerRun>();
        playerJump = GetComponent<PlayerJump>();
    }


    public void ChangeAnimationState(string newState)
    {
        if (currentStringState == newState) return;
        animator.Play(newState);

        currentStringState = newState;
    }

    void Update()
    {

        //====== Di chuyển =========
        MoveX = 0;

        if (Input.GetKey(KeyBiding.leftMove))
            MoveX = -1;
        else if (Input.GetKey(KeyBiding.rightMove))
            MoveX = 1;

        //======= Nhảy ==========
        if (Input.GetKeyDown(KeyBiding.jumpKey))
        {
            IsJumpPressed = true;
        }
    }
}
