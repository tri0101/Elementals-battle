using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroControl : Subject
{
    [Header("Attribute")]
    
    
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
    [SerializeField] private Vector3 battleTarget;
    public Vector3 BattleTarget => battleTarget;

    [SerializeField] private bool needMoveToBattle;
    public bool NeedMoveToBattle => needMoveToBattle;

    [SerializeField] private string currentStringState;
    public string CurrentStringState => currentStringState;

    [Header("Other Script")]

    HeroStateManager heroStateManager;
    public HeroStateManager HeroStateManager => heroStateManager;

    HeroRun heroRun;
    public HeroRun HeroRun => heroRun;

    HeroAttackk heroAttackk;
    public HeroAttackk HeroAttackk => heroAttackk;
   
    HeroIdle heroIdle;
    public HeroIdle HeroIdle => heroIdle;
    HeroSkilll heroSkilll;
    public HeroSkilll heroSkill => heroSkilll;

    HeroReceiveDamagee heroReceiveDamagee;
    public HeroReceiveDamagee HeroReceiveDamagee => heroReceiveDamagee;
    HeroEventt heroEventt;
    public HeroEventt HeroEventt => heroEventt;


    [Header("ScriptableObject")]
    [SerializeField] private HeroInfo heroInfo;
    public HeroInfo HeroInfo => heroInfo;



    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    public Transform Enemy => enemy;



    public const string SkillObserver = "Skill";
   
    private void Awake()
    {



        normalT = transform.Find("Normal");
        receiveT = normalT.Find("ColliderReceive");
        fixedLayer = LayerMask.LayerToName(receiveT.gameObject.layer);
        animator = normalT.GetComponent<Animator>();
        spriteRenderer = normalT.GetComponent<SpriteRenderer>();

     
        
        heroStateManager = GetComponent<HeroStateManager>();
        heroRun = GetComponent<HeroRun>();
      
        heroAttackk = GetComponent<HeroAttackk>();

   
        heroIdle = GetComponent<HeroIdle>();
        heroSkilll = GetComponent<HeroSkilll>();
      
        heroReceiveDamagee = transform.GetChild(0).Find("ColliderReceive").GetComponent<HeroReceiveDamagee>();
        heroEventt = transform.GetChild(0).GetComponent<HeroEventt>();
        
        //string targetTag = CompareTag("Player1") ? "Player2" : "Player1";
        //GameObject enemyObj = GameObject.FindGameObjectWithTag(targetTag);
        //enemy = enemyObj.transform;
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
        
        
       
    }
    public float  GetMoveX()
    {
        if(currentStringState != "Run")
        {
            return 0;
        }

        else
        {
           if(transform.localScale.x > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
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

  
  
    public void RefreshObservers()
    {
        NotifyObservers();
    }
   
    public void RefreshObservers(object data1)
    {
        NotifyObservers(data1);
    }
    
   
    string lastStateName;

    void CheckName()
    {
        if (currentStringState == lastStateName) return;

        Debug.Log("State changed: " + currentStringState);
        lastStateName = currentStringState;
    }
    //khi vào trận , dc gọi từ battle manager
    public void SetBattleTarget(Vector3 pos)
    {
        battleTarget = pos;
        needMoveToBattle = true;
    }
    // hàm đã đến đích , gọi từ heroRunState
    public void SetArrivedBattle()
    {
        needMoveToBattle = false;
        
    }
}
