using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField] private bool isJumpPressed;

    public bool IsJumpPressed
    {
        get => isJumpPressed;
        set => isJumpPressed = value;
    }
    [SerializeField] private bool isAttackPressed;
    public bool IsAttackPressed
    {
        get => isAttackPressed;
        set => isAttackPressed = value;
    }
    [SerializeField] private string currentStringState;
    public string CurrentStringState => currentStringState;

    [Header("Other Script")]
    PlayerCheckingGround playerCheckingGround;

    public PlayerCheckingGround PlayerCheckingGround => playerCheckingGround;
    PlayerStateManager playerStateManager;
    public PlayerStateManager PlayerStateManager => playerStateManager;

    PlayerRun playerRun;
    public PlayerRun PlayerRun => playerRun;
    PlayerJump playerJump;
    public PlayerJump PlayerJump => playerJump;
    PlayerAttackk playerAttackk;
    public PlayerAttackk PlayerAttackk => playerAttackk;

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
        playerAttackk = GetComponent<PlayerAttackk>();
    }


    public void ChangeAnimationState(string newState, float time = 0)
    {
        if (time > 0) StartCoroutine(Wait());
        else Validate();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);
            Validate();

        }

        void Validate()
        {
            if (currentStringState == newState) return;
            animator.Play(newState);

            currentStringState = newState;
        }
        
    }

    public void ChangeAnimationStateLoop(string newState)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        
        if (currentStringState == newState)
        {
            
            if (info.normalizedTime < 1f)
                return;

            
            animator.Play(newState, 0, 0f);
            return;
        }

       
        animator.Play(newState, 0, 0f);
        currentStringState = newState;
    }
    public void CheckAnimation()
    {

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
        if (Input.GetKeyDown(KeyBiding.jumpKey) && !currentStringState.StartsWith("Jump"))
        {
            IsJumpPressed = true;
        }
        if (Input.GetKeyDown(keyBiding.attackKey))
        {
            isAttackPressed = true;
        }
    }

   
}
