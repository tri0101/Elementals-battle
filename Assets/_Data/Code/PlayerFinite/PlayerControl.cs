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
    [SerializeField] private bool isSkillPressed;
    public bool IsSkillPressed
    {
        get => isSkillPressed;
        set => isSkillPressed = value;
    }
    [SerializeField] private bool isBlockPressed;
    public bool IsBlockPressed
    {
        get => isBlockPressed;
        set => isBlockPressed = value;
    }
    [SerializeField] private bool canBlock;
    public bool CanBlock
    {
        get => canBlock;
        set => canBlock = value;
    }

    
    [SerializeField] private bool isTransformPressed;
    public bool IsTransformPressed
    {
        get => isTransformPressed;
        set => isTransformPressed = value;
    }
    [SerializeField] private bool hasBeenTransform;
    public bool HasBeenTransform
    {
        get => hasBeenTransform;
        set => hasBeenTransform = value;
    }
    [SerializeField] private bool isRangedAttackPressed;
    public bool IsRangedAttackPressed
    {
        get => isRangedAttackPressed;
        set => isRangedAttackPressed = value;
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
    PlayerTransformm playerTransformm;
    public PlayerTransformm PlayerTransformm => playerTransformm;

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
        playerTransformm = GetComponent<PlayerTransformm>();
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

    void Update()
    {
        CheckName();
        //====== Di chuyển =========
        MoveX = 0;

        if (Input.GetKey(KeyBiding.leftMove))
            MoveX = -1;
        else if (Input.GetKey(KeyBiding.rightMove))
            MoveX = 1;


        //isBlockPressed = Input.GetKey(keyBiding.blockKey);


        if (currentStringState.Contains("Block") && Input.GetKeyUp(keyBiding.blockKey))
        {
            isBlockPressed = false;
        }
        //Lần đánh 2 , 3
        if (Input.GetKeyDown(keyBiding.attackKey) && currentStringState.Contains("Attack"))
        {
            isAttackPressed = true;
        }
        //Air attack
        if (Input.GetKeyDown(keyBiding.attackKey) && currentStringState.Contains("Jump"))
        {
            isAttackPressed = true;
        }
        if (!currentStringState.Contains("Idle") && !currentStringState.Contains("Run")) return;

       
        //======= Nhảy ==========
        if (Input.GetKeyDown(KeyBiding.jumpKey) && !currentStringState.StartsWith("Jump"))
        {
            IsJumpPressed = true;
        }
        if (Input.GetKeyDown(KeyBiding.blockKey) && canBlock)
        {
            isBlockPressed = true;
            canBlock = false;
        }
        if (Input.GetKeyDown(keyBiding.transformKey) && !hasBeenTransform)
        {
            isTransformPressed = true;

        }
        if (Input.GetKeyDown(KeyBiding.rangedAttackKey) && !hasBeenTransform)
        {
            isRangedAttackPressed = true;
        }
        //Lần đánh 1
        if (Input.GetKeyDown(keyBiding.attackKey))
        {
            isAttackPressed = true;
        }


        if (Input.GetKeyDown(keyBiding.skillKey))
        {
            isSkillPressed = true;
        }

    }
    
    // index = 1 : so sánh lớn hơn duration , index = 0 : ngược lại
    public bool CheckCurrentAnimation(string stateCheck, float duration, int index)
    {
        Animator anim = animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if(index == 1)
        {
            return ((info.IsName(stateCheck) && info.normalizedTime >= duration));
        }
        else
        {
            return ((info.IsName(stateCheck) && info.normalizedTime <= duration));
        }
     
    }
    public bool CheckCurrentAnimationName(string nameAnim)
    {

        Animator anim = animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        return (info.IsName(nameAnim));
    }
    //TEst thôi
    void SpriteName()
    {
        if (spriteRenderer.sprite == null) return;

        Debug.Log("Current Sprite: " + spriteRenderer.sprite.name);
    }
    string lastStateName;

    void CheckName()
    {
        if (currentStringState == lastStateName) return;

        Debug.Log("State changed: " + currentStringState);
        lastStateName = currentStringState;
    }

}
