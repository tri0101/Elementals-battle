using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

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
    [SerializeField] private bool isPrepare;
    public bool IsPrepare
    {
        get => isPrepare;
        set => isPrepare = value;
    }
    [SerializeField] private bool isAttack;
    public bool IsAttack
    {
        get => isAttack;
        set => isAttack = value;
    }

    [SerializeField] private bool isUltimate;
    public bool IsUltimate
    {
        get => isUltimate;
        set => isUltimate = value;
    }

    [SerializeField] private bool isSkill;
    public bool IsSkill
    {
        get => isSkill;
        set => isSkill = value;
    }
    [SerializeField] private bool isFinished;
    public bool IsFinished
    {
        get => isFinished;
        set => isFinished = value;
    }

    [SerializeField] private Vector3 battleTarget;
    public Vector3 BattleTarget => battleTarget;

    [SerializeField] private bool needMoveToBattle;
    public bool NeedMoveToBattle => needMoveToBattle;

    [SerializeField] private string currentStringState;
    public string CurrentStringState => currentStringState;

    [Header("Other Script")]
    HeroStatRuntime heroStatRuntime;
    public HeroStatRuntime HeroStatRuntime => heroStatRuntime;

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
    public List<Transform> enemyTarget = new List<Transform>();

    public const string SkillObserver = "Skill";

    // =============================
    //  Turn manager synchronization
    // =============================
    private bool actionInProgress;
    public bool ActionInProgress => actionInProgress;
   

    /// <summary>
    /// Called by animation event at the end of Attack/Skill/Ultimate.
    /// BattleTurnManager will wait until ActionInProgress becomes false.
    /// </summary>
    public void NotifyActionFinished()
    {
        actionInProgress = false;
    }

    // =============================
    //  Requests from BattleTurnManager
    // =============================

    public void SetAttack()
    {
        isAttack = true;
        actionInProgress = true;

        // For now: ONLY handle normal attack targeting
        BuildTargetsForNormalAttack();
    }

    public void SetUltimate()
    {
        isUltimate = true;
        actionInProgress = true;
        // TODO: BuildTargetsForUltimate()
    }

    public void SetSkill()
    {
        isSkill = true;
        actionInProgress = true;
        // TODO: BuildTargetsForSkill()
    }

    private void BuildTargetsForNormalAttack()
    {
        enemyTarget.Clear();

        if (BattlefieldRegistry.Instance == null)
            return;

        // Determine which team is enemy
        string enemyTag = CompareTag("Hero") ? "Enemy" : "Hero";

        // Prefer random in FRONT column (slots 1..3). If none alive, random in BACK column (slots 4..6).
        var front = GetAliveEnemiesInColumn(enemyTag, 1);
        Transform chosen = null;

        if (front.Count > 0)
        {
            chosen = front[Random.Range(0, front.Count)];
        }
        else
        {
            var back = GetAliveEnemiesInColumn(enemyTag, 2);
            if (back.Count > 0)
                chosen = back[Random.Range(0, back.Count)];
        }

        if (chosen != null)
            enemyTarget.Add(chosen);
    }

    private static List<Transform> GetAliveEnemiesInColumn(string enemyTag, int columnIndex)
    {
        var result = new List<Transform>(6);

        var roots = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTag);
        for (int i = 0; i < roots.Count; i++)
        {
            var root = roots[i];
            if (root == null) continue;

            if (!BattlefieldRegistry.Instance.TryGetSlotIndex(root, out int slot)) continue;
            if (BattlefieldRegistry.SlotToColumn(slot) != columnIndex) continue;

            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            result.Add(root);
        }

        return result;
    }

    private void Awake()
    {
        normalT = transform.Find("Normal");
        receiveT = normalT.Find("ColliderReceive");
        fixedLayer = LayerMask.LayerToName(receiveT.gameObject.layer);
        animator = normalT.GetComponent<Animator>();
        spriteRenderer = normalT.GetComponent<SpriteRenderer>();

        heroStatRuntime = GetComponent<HeroStatRuntime>();
        heroStateManager = GetComponent<HeroStateManager>();
        heroRun = GetComponent<HeroRun>();

        heroAttackk = GetComponent<HeroAttackk>();

        heroIdle = GetComponent<HeroIdle>();
        heroSkilll = GetComponent<HeroSkilll>();

        heroReceiveDamagee = transform.GetChild(0).Find("ColliderReceive").GetComponent<HeroReceiveDamagee>();
        heroEventt = transform.GetChild(0).GetComponent<HeroEventt>();
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
            animator.Play(newState, 0, 0f);
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

 

    public bool CheckCurrentAnimation(string stateCheck, float duration, int index)
    {
        Animator anim = animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (index == 1)
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

    public void ChangeLayerAtReceive(string layerName)
    {
        receiveT.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

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
    public void RefreshObservers(HeroNotifyType type, object data = null)
    {
        NotifyObservers(type, data);
    }

    public void SetBattleTarget(Vector3 pos)
    {
        battleTarget = pos;
        needMoveToBattle = true;
    }

    public void SetArrivedBattle()
    {
        needMoveToBattle = false;
    }
    public void GoBackBattleTarget()
    {
        needMoveToBattle = true;
    }
}