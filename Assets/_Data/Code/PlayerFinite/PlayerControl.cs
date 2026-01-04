using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : Subject
{
    [Header("Attribute")]
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    [SerializeField] private string fixedLayer;
    public string FixedLayer => fixedLayer; 
    [SerializeField] private Transform normalT;
    public Transform NormalT => normalT;
  
    [SerializeField] private Transform receiveT;
    public Transform ReceiveT => receiveT;
  
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
    [SerializeField] private bool hasAttackedWhenJump;
    public bool HasAttackedWhenJump
    {
        get => hasAttackedWhenJump;
        set => hasAttackedWhenJump = value;
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
    [SerializeField] private bool isRollPressed;
    public bool IsRollPressed
    {
        get => isRollPressed;
        set => isRollPressed = value;
    }
    public bool WasInAirBeforeHit;
    public bool WasFallingBeforeHit;
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
    PlayerRoll playerRoll;
    public PlayerRoll PlayerRoll => playerRoll;
    PlayerIdle playerIdle;
    public PlayerIdle PlayerIdle => playerIdle;
    PlayerSkilll playerSkilll;
    public PlayerSkilll PlayerSkilll => playerSkilll;
    PlayerReceiveDamagee playerReceiveDamagee;
    public PlayerReceiveDamagee PlayerReceiveDamagee => playerReceiveDamagee;
    PlayerEventt playerEventt;
    public PlayerEventt PlayerEventt => playerEventt;
    PlayerInput playerInput;
    public PlayerInput PlayerInput => playerInput;

    [Header("ScriptableObject")]
    [SerializeField] private PlayerInfo playerInfo;
    public PlayerInfo PlayerInfo => playerInfo;
    [Header("KeyBiding")]
    [SerializeField] KeyBinding keyBiding;
    public KeyBinding KeyBiding => keyBiding;


    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    public Transform Enemy => enemy;

    private void Awake()
    {
        normalT = transform.Find("Normal");
        receiveT = normalT.Find("ColliderReceive");
        fixedLayer = LayerMask.LayerToName(receiveT.gameObject.layer);
        animator = normalT.GetComponent<Animator>();
        spriteRenderer = normalT.GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        playerCheckingGround = GetComponent<PlayerCheckingGround>();
        playerStateManager = GetComponent<PlayerStateManager>();
        playerRun = GetComponent<PlayerRun>();
        playerJump = GetComponent<PlayerJump>();
        playerAttackk = GetComponent<PlayerAttackk>();
        playerTransformm = GetComponent<PlayerTransformm>();
        playerRoll = GetComponent<PlayerRoll>();    
        playerIdle = GetComponent<PlayerIdle>();
        playerSkilll = GetComponent<PlayerSkilll>();
        playerReceiveDamagee = transform.GetChild(0).Find("ColliderReceive").GetComponent<PlayerReceiveDamagee>();
        playerEventt = transform.GetChild(0).GetComponent<PlayerEventt>();
        playerInput = GetComponent<PlayerInput>();
        string targetTag = CompareTag("Player1") ? "Player2" : "Player1";
        GameObject enemyObj = GameObject.FindGameObjectWithTag(targetTag);
        enemy = enemyObj.transform;
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

    //cho take hit
    public void ChangeAnimationAnyState(string newState, float time = 0)
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
           
           
            animator.Play(newState,0,0f);
            animator.speed = 1f;

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
        //if (PlayerReceiveDamagee.IsDead) return;
        //====== Di chuyển =========
        MoveX = 0;

    
        if ((playerInput.isLeftMove || playerInput.isLeftMovePC) && !currentStringState.Contains("Block"))
            MoveX = -1;
        else if ((playerInput.isRightMove || playerInput.isRightMovePC) && !currentStringState.Contains("Block"))
            MoveX = 1;


        //isBlockPressed = Input.GetKey(keyBiding.blockKey);
        
        //Check bị đánh lần đầu
        if (playerReceiveDamagee.IsHit)
        {
            
            WasInAirBeforeHit = !playerCheckingGround.IsGrounded;
            WasFallingBeforeHit = rb.linearVelocity.y <= 0;

            
            playerReceiveDamagee.IsHit = false;




            playerStateManager.SwitchAnyState(playerStateManager.takeHitState);




            return;
        }
       

        if (currentStringState.Contains("Block") && ((playerInput.isBlockInputUp || playerInput.isBlockInputUpPC)))
        {
            isBlockPressed = false;
        }
        //Lần đánh 2 , 3
        if (playerInput.isAttackInput && !hasAttackedWhenJump && currentStringState.Contains("Attack"))
        {
            isAttackPressed = true;
           
        }
        //Air attack
        if (playerInput.isAttackInput && currentStringState.Contains("Jump") && !hasAttackedWhenJump)
        {
            isAttackPressed = true;
        }
        if (!currentStringState.Contains("Idle") && !currentStringState.Contains("Run")) return;

       
        //======= Nhảy ==========
        if (playerInput.isJumpInput && !currentStringState.StartsWith("Jump"))
        {
            IsJumpPressed = true;
        }
        if ((playerInput.isBlockInputDown || playerInput.isBlockInputDownPC) && canBlock)
        {
            isBlockPressed = true;
            canBlock = false;
        }
        if (playerInput.isTransformInput && !hasBeenTransform && playerReceiveDamagee.Mana >= 1000f)
        {
            isTransformPressed = true;

        }
        if (playerInput.isRollInput && !hasBeenTransform)
        {
            isRollPressed = true;

        }
        if (playerInput.isRangedAttackInput && !hasBeenTransform )
        {
            isRangedAttackPressed = true;
        }
        //Lần đánh 1
        if (playerInput.isAttackInput)
        {
            isAttackPressed = true;
        }


        if (playerInput.isSkillInput && playerReceiveDamagee.Mana >= 500f) 
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



    //Đổi sang các layer khác
    public void ChangeLayerAtReceive(string layerName )
    {
        receiveT.gameObject.layer = LayerMask.NameToLayer(layerName);
    }
    //trả về layer mặc định
    public void ReturnFixedLayer()
    {
        receiveT.gameObject.layer = LayerMask.NameToLayer(fixedLayer);
    }

    public void CheckUnder()
    {
        if (playerReceiveDamagee.IsDead) return;
        if (currentStringState.Contains("Roll")) return;
        if (playerCheckingGround.IsUnderPlayer)
        {
            Vector3 pos = transform.position;
            if (playerCheckingGround.IsWallLeft)
            {
                pos.x += 1.5f;
                
            }
            else if(playerCheckingGround.IsWallRight) 
            {
                pos.x -= 1.5f;
            }
                
            else if (transform.localScale.x > 0)
            {
                pos.x -= 1.5f;
            }
            else
            {
                pos.x += 1.5f;
            }
            transform.position = pos;
        }
    }
    private void FixedUpdate()
    {
        CheckUnder();
    }

    public void RefreshObservers()
    {
        NotifyObservers();
    }
    public void RefreshObserversThis()
    {
        NotifyObservers(this);
    }
    public void AutoFlip()
    {

        if (MoveX != 0) return;
        float baseScaleX = Mathf.Abs(transform.localScale.x);
        if (transform.localPosition.x > Enemy.transform.localPosition.x)
        {
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }
        else
        {

            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }
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
