using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;

public class HeroControl : Subject
{
    [Header("Attribute")]
    [SerializeField] CapsuleCollider2D capsuleCollider2D;
    public CapsuleCollider2D CapsuleCollider2D => capsuleCollider2D;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    [SerializeField] private SpriteRenderer spriteEffect;
    public SpriteRenderer SpriteEffect => spriteEffect;
    [SerializeField] private Transform listEffect;
    public Transform ListEffect => listEffect;
    [SerializeField] private UI_CanvasTotalDamage canvasTotalDamage;
    public UI_CanvasTotalDamage CanvasTotalDamage => canvasTotalDamage;

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
    [SerializeField] private bool isUltimateSpecial;
    public bool IsUltimateSpecial
    {
        get => isUltimateSpecial;
        set => isUltimateSpecial = value;
    }
    [SerializeField] bool canUltimateSpecial;
    public bool CanUltimateSpecial
    {
        get => canUltimateSpecial;
        set => canUltimateSpecial = value;
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
    [SerializeField] private bool canSkill;
    public bool CanSkill
    {
        get => canSkill;
        set => canSkill = value;
    }
    [SerializeField] private bool canDodge;
    public bool CanDodge
    {
        get => canDodge;
        set => canDodge = value;
    }
    [SerializeField] private bool isTakeHit;
    public bool IsTakeHit
    {
        get => isTakeHit;
        set => isTakeHit = value;
    }
    [SerializeField] private bool isDead = false;
    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }
    [SerializeField] private bool isDodge = false;
    public bool IsDodge
    {
        get => isDodge;
        set => isDodge = value;
    }
    [SerializeField] private bool isClear = false;
    public bool IsClear
    {
        get => isClear;
        set => isClear = value;
    }
    [SerializeField] private bool isCrit = false;
    public bool IsCrit
    {
        get => isCrit;
        set => isCrit = value;
    }
    [SerializeField] private bool shouldPlus = false; // dùng để check xem có thêm duration ko ( cân bằng cho hero đánh sau )
    public bool ShouldPlus
    {
        get => shouldPlus;
        set => shouldPlus = value;
    }
    [SerializeField] private bool leftBattle = false;// trốn khỏi battle
    public bool LeftBattle
    {
        get => leftBattle;
        set => leftBattle = value;
    }
    [SerializeField] private bool isTransform;
    public bool IsTransform
    {
        get => isTransform;
        set => isTransform = value;
    }
    [SerializeField] private bool isStart;// true = đánh trước
    public bool IsStart
    {
        get => isStart;
        set => isStart = value;
    }
    [SerializeField] private bool isSpawn;// true = đánh trước
    public bool IsSpawn
    {
        get => isSpawn;
        set => isSpawn = value;
    }
    [SerializeField] private Vector3 battleTarget;
    public Vector3 BattleTarget => battleTarget;

    [SerializeField] private Transform clearPosition;// di chuyển đến vị trí này khi clea
    public Transform ClearPosition => clearPosition;

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
    public HeroSkilll HeroSkill => heroSkilll;
    HeroUltimate heroUltimate;
    public HeroUltimate HeroUltimate => heroUltimate;

    HeroReceiveDamagee heroReceiveDamagee;
    public HeroReceiveDamagee HeroReceiveDamagee => heroReceiveDamagee;

    HeroEventt heroEventt;
    public HeroEventt HeroEventt => heroEventt;
    HeroDodge heroDodge;
    public HeroDodge HeroDodge => heroDodge;
    HeroTransform heroTransform;
    public HeroTransform HeroTransform => heroTransform;
    [Header("ScriptableObject")]
    [SerializeField] private HeroInfo heroInfo;
    public HeroInfo HeroInfo => heroInfo;

    [Header("Enemy")]
    public List<Transform> enemyTarget = new List<Transform>();
    public Vector3 distanceToTarget;
    public const string SkillObserver = "Skill";




    [SerializeField] private bool canAttackInBattle = true;
    public bool CanAttackInBattle
    {
        get => canAttackInBattle;
        set => canAttackInBattle = value;
    }
    public void Start()
    {
        canAttackInBattle = true;
        InitClearPosition();
    }
    void InitClearPosition()
    {
        GameObject clearObj = new GameObject($"{name}_ClearPosition");
        clearObj.transform.SetParent(transform, false); 

        clearObj.transform.position = new Vector3(
            50f,
            transform.position.y,
            transform.position.z
        );

        clearPosition = clearObj.transform;
    }
    //public void NotifyActionFinished()
    //{
    //    actionInProgress = false;
    //}
    public void SetClear()
    {
        isClear = true;
        enemyTarget.Clear();
        
        distanceToTarget = clearPosition.position;
    }

    public void SetTransform()
    {
        isTransform = true;
    }
    //tính tỉ lệ chí mạng
    public bool IsCritical()
    {
        float totalCritRate = heroStatRuntime.CritRate + heroStatRuntime.GetTotalModifyPercent(ModifyStatType.CritRate);
        float randomValue = Random.Range(0f, 100f);
        return randomValue < totalCritRate;
    }
    public void SetAttack()
    {
        if(heroInfo.normalAttack == null)
            return;

     
        isAttack = true;
        isCrit = IsCritical();

        
    }
    public void SetShouldPlus(bool value)
    {
        shouldPlus = value;
    }
    public void SetCanDodge()
    {
        canDodge = true;
        IsDodge = true;
        leftBattle = true;
    }
    public void SetUltimate()
    {
        if(heroInfo.ultimate == null)
            return;
        isUltimate = true;
        isCrit = IsCritical();
  
        
    }
    
    public void SetUltimateSpecial()
    {
        if(heroInfo.ultimateSpecial == null)
            return;

        isUltimateSpecial = true;
        isCrit = IsCritical();
  
        
    }
    
    public void SetTarget(AbilityInfo info)
    {
        

        needMoveToBattle = false;
        if (!(heroInfo.ID == 8 && info.type == AbilityType.Skill))
            BuildTargets(info);
        if (!(heroInfo.ID == 507 && info.type == AbilityType.Ultimate))
            BuildTargets(info);
        distanceToTarget = GetAttackPosition(info);



    }
    public void SetTarget(List<Transform> targets, AbilityInfo info)// khi muốn set cụ thể
    {
        enemyTarget = targets;
        distanceToTarget = GetAttackPosition(info);
    }

    public void SetSkill()
    {
        if(heroInfo.skill == null)
            return;
  
        isSkill = true;
        isCrit = IsCritical();

        
        
    }


    public void SetIsTakeHit()
    {
        isTakeHit = true;

    }
    public void SetIsDead()
    {
        if (heroInfo.ID == 58)
        {
            isDead = false;
        }
        else isDead = true;
        

    }
    public void CheckCanSpecial()
    {
        if(heroInfo.ID == 5)
        {
            canUltimateSpecial = (HasAnyEnemyHpPercentBelow(0.25f)); 
           
        }
       
    }
    private void BuildTargets(AbilityInfo ability)
    {
        enemyTarget.Clear();

        if (BattlefieldRegistry.Instance == null)
            return;

        //if (heroInfo == null || heroInfo.normalAttack == null)
        //{
            
        //    BuildTargetsForNormalAttack_NoneModeFallback();
        //    return;
        //}

        
        string enemyTeam = CompareTag("Hero") ? "Enemy" : "Hero";

        switch (ability.targetingMode)
        {
            
            case AbilityTargetingMode.None:
                BuildTargetsNone();
                break;
            case AbilityTargetingMode.LowestHp:
                
                BuildTargetLowestHP();
                break;

           case AbilityTargetingMode.Row:
                {
                    // Ưu tiên hàng trước: Column 1 (slot 1/2/3).
                    // Nếu còn sống => random 1 con trong 1/2/3 rồi lấy CẢ row của nó (vd slot 2 => row (2,5)).
                    var aliveFront = GetAliveEnemiesInColumn(enemyTeam, 1);
                    
                    if (aliveFront.Count > 0)
                    {
                        Transform chosen = aliveFront[Random.Range(0, aliveFront.Count)];
                        Transform mainTarget = chosen;
                        if (BattlefieldRegistry.Instance.TryGetSlotIndex(chosen, out int slot))
                        {
                            int row = BattlefieldRegistry.SlotToRow(slot);
                            enemyTarget.Clear();
                            AddAliveEnemiesInRow(enemyTeam, row);
                        }
                        else
                        {
                            BuildTargetsNone();
                        }

                        break;
                    }

                    // Hàng trước chết hết => mới xét hàng sau: Column 2 (slot 4/5/6)
                    var aliveBack = GetAliveEnemiesInColumn(enemyTeam, 2);

                    if (aliveBack.Count > 0)
                    {
                        Transform chosen = aliveBack[Random.Range(0, aliveBack.Count)];

                        if (BattlefieldRegistry.Instance.TryGetSlotIndex(chosen, out int slot))
                        {
                            int row = BattlefieldRegistry.SlotToRow(slot);
                            enemyTarget.Clear();
                            AddAliveEnemiesInRow(enemyTeam, row);
                        }
                        else
                        {
                            BuildTargetsNone();
                        }

                        break;
                    }

                    // Không còn mục tiêu
                    BuildTargetsNone();
                    break;
                }
            case AbilityTargetingMode.AoeAllEnemies:
                {
                    // FIX: AoE lấy toàn bộ enemy còn sống
                    var aliveEnemies = AddAliveEnemiesAll(enemyTeam);

                    // AddAliveEnemiesAll currently returns a list but doesn't push into enemyTarget.
                    // So we add them into enemyTarget here.
                    for (int i = 0; i < aliveEnemies.Count; i++)
                        enemyTarget.Add(aliveEnemies[i]);

                    break;
                }


            case AbilityTargetingMode.Column:
                {
                    
                    int col = ability.column == ColumnTarget.Back ? 2 : 1;
                    AddAliveEnemiesInColumn(enemyTeam, col);
                    break;
                }

            case AbilityTargetingMode.HighestAttack:
                {
                    var aliveEnemies = AddAliveEnemiesAll(enemyTeam);

                    Debug.Log($"[HighestAttack] Found {aliveEnemies.Count} alive enemies");

                    if (aliveEnemies.Count == 0)
                    {
                        Debug.Log("[HighestAttack] No enemies found, building none");
                        BuildTargetsNone();
                        break;
                    }

                    Transform highestDamageEnemy = null;
                    float maxDamage = -1f;

                    foreach (Transform enemy in aliveEnemies)
                    {
                        HeroControl enemyControl = enemy.GetComponent<HeroControl>();
                        if (enemyControl == null || enemyControl.HeroStatRuntime == null)
                        {
                            Debug.Log($"[HighestAttack] {enemy.name} - control or runtime null");
                            continue;
                        }

                        float baseDamage = enemyControl.HeroStatRuntime.Damage;
                        float totalDamage = enemyControl.HeroStatRuntime.GetFinalValueAfterModifyStat(
                            ModifyStatType.Damage,
                            baseDamage
                        );

                        Debug.Log($"[HighestAttack] {enemy.name} - baseDamage={baseDamage}, totalDamage={totalDamage}");

                        if (totalDamage > maxDamage)
                        {
                            maxDamage = totalDamage;
                            highestDamageEnemy = enemy;
                        }
                    }

                    Debug.Log($"[HighestAttack] Selected: {(highestDamageEnemy != null ? highestDamageEnemy.name : "NONE")}, maxDamage={maxDamage}");

                    if (highestDamageEnemy != null)
                        enemyTarget.Add(highestDamageEnemy);
                    else
                        BuildTargetsNone();

                    break;
                }
        }
    }
    public Vector3 GetAttackPosition(AbilityInfo ability)
    {
        if(heroInfo.ID != 8 && enemyTarget.Count == 0)
        {
            isFinished = true;
        }
        if (heroInfo.ID != 8  && (enemyTarget == null || enemyTarget.Count == 0))
        {
            Debug.Log("enemy target null or empty");
            return transform.position;
        }
        


        Vector3 result = transform.position;

        switch (ability.positionAttack)
        {
            case PositionAttack.CurrentPosition:
                result = transform.position;
                break;
            case PositionAttack.MiddlePosition:
                // (0, 2.252, z giữ nguyên)
                result = new Vector3(
                    0f,
                    2.252f,
                    transform.position.z
                );
                break;
            case PositionAttack.MiddlePositionEnemy:
                // (0, 2.252, z giữ nguyên)
                result = new Vector3(
                    16.1f,
                    2.252f,
                    transform.position.z
                );
                break;
           
            case PositionAttack.DistanceToTarget:
                Transform enemy = enemyTarget[0];
                // đứng trước enemy 1 khoảng = distance
                Debug.Log(enemy.name + " position: " + enemy.position.x);
                float dir = enemy.position.x > transform.position.x ? -1f : 1f;

                result = enemy.position;
                result.x += dir * ability.distance;
                break;

            case PositionAttack.MiddleRow:
                {
                    
                    if (enemyTarget.Count == 1)
                    {
                        // Nếu chỉ có 1 enemy thì đứng trước nó
                        Transform enemyOnly = enemyTarget[0];
                        float direc = enemyOnly.position.x > transform.position.x ? -1f : 1f;

                        result = enemyOnly.position;
                        result.x += direc * ability.distance;
                        return result;
                    }

                    // Lấy 2 enemy đầu trong row
                    Transform enemyA = enemyTarget[0];
                    Transform enemyB = enemyTarget[1];
                    Debug.Log(enemyA.name + enemyA.position.x);
                    Debug.Log(enemyB.name + enemyB.position.x);
                    Debug.Log("Enemy world pos: " + enemyA.position.x);
                    Debug.Log("Enemy local pos: " + enemyA.localPosition.x);
                    Debug.Log("Enemy parent pos: " + enemyA.parent.position.x);
                    Debug.Log("Enemy world pos: " + enemyB.position.x);
                    Debug.Log("Enemy local pos: " + enemyB.localPosition.x);
                    Debug.Log("Enemy parent pos: " + enemyB.parent.position.x);
                    // Tính trung điểm
                    float middleX = (enemyA.position.x + enemyB.position.x) / 2f;
                    Debug.Log($"Middle X: {middleX}");

                    result = new Vector3(
                        middleX,
                        enemyA.position.y,
                        transform.position.z
                    );

                    break;
                 }
            

        }

        return result;
    }
    private void BuildTargetLowestHP()
    {
        Transform enemyTeam = GetEnemyTransformWithLowestHpPercent();
        if(enemyTeam != null)
        {
            enemyTarget.Add(enemyTeam);
        }
    }
    private void BuildTargetsNone()
    {
        enemyTarget.Clear();

        if (BattlefieldRegistry.Instance == null)
            return;

        string enemyTeam = CompareTag("Hero") ? "Enemy" : "Hero";

        // Prefer random in FRONT column (slots 1..3). If none alive, random in BACK column (slots 4..6).
        var front = GetAliveEnemiesInColumn(enemyTeam, 1);
        Transform chosen = null;

        if (front.Count > 0)
        {
            chosen = front[Random.Range(0, front.Count)];
        }
        else
        {
            var back = GetAliveEnemiesInColumn(enemyTeam, 2);
            if (back.Count > 0)
                chosen = back[Random.Range(0, back.Count)];
        }

        if (chosen != null)
            enemyTarget.Add(chosen);
    }

    private List<Transform> AddAliveEnemiesAll(string enemyTeam)
    {
        var result = new List<Transform>(6);
        var roots = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);
        for (int i = 0; i < roots.Count; i++)
        {
            var root = roots[i];
            if (root == null) continue;
            var heroControl = root.GetComponent<HeroControl>();
            if (heroControl != null && heroControl.LeftBattle) continue;
            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            result.Add(root);
        }
        return result;
    }

    private void AddAliveEnemiesInRow(string enemyTeam, int rowIndex1To3)
    {
        var roots = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);
        for (int i = 0; i < roots.Count; i++)
        {
            var root = roots[i];
            if (root == null) continue;

            if (!BattlefieldRegistry.Instance.TryGetSlotIndex(root, out int slot)) continue;
            if (BattlefieldRegistry.SlotToRow(slot) != rowIndex1To3) continue;
            var heroControl = root.GetComponent<HeroControl>();
            if (heroControl != null && heroControl.LeftBattle) continue;
            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            enemyTarget.Add(root);
        }
    }

    private void AddAliveEnemiesInColumn(string enemyTeam, int columnIndex1To2)
    {
        var roots = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);
        for (int i = 0; i < roots.Count; i++)
        {
            var root = roots[i];
            if (root == null) continue;

            if (!BattlefieldRegistry.Instance.TryGetSlotIndex(root, out int slot)) continue;
            if (BattlefieldRegistry.SlotToColumn(slot) != columnIndex1To2) continue;
            var heroControl = root.GetComponent<HeroControl>();
            if (heroControl != null && heroControl.LeftBattle) continue;
            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            enemyTarget.Add(root);
        }
    }

    private static List<Transform> GetAliveEnemiesInColumn(string enemyTeam, int columnIndex)
    {
        var result = new List<Transform>(6);

        var roots = BattlefieldRegistry.Instance.GetUnitsByTeam(enemyTeam);
        for (int i = 0; i < roots.Count; i++)
        {
            var root = roots[i];
            if (root == null) continue;

            if (!BattlefieldRegistry.Instance.TryGetSlotIndex(root, out int slot)) continue;
            if (BattlefieldRegistry.SlotToColumn(slot) != columnIndex) continue;
            var heroControl = root.GetComponent<HeroControl>();
            if(heroControl != null && heroControl.LeftBattle) continue;
            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;
            
            result.Add(root);
        }

        return result;
    }
    public bool HasAnyEnemyHpPercentBelow(float value)
    {
        if (BattlefieldRegistry.Instance == null || HeroStatRuntime == null)
            return false;

        float threshold = Mathf.Clamp01(value);
        string teamTag = CompareTag("Hero") ? "Enemy" : "Hero";

        var allies = BattlefieldRegistry.Instance.GetUnitsByTeam(teamTag);
        for (int i = 0; i < allies.Count; i++)
        {
            var root = allies[i];
            if (root == null) continue;

            var ally = root.GetComponent<HeroControl>();
            if (ally == null) continue;
            if (ally.LeftBattle) continue;

            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            if (ally.HeroStatRuntime == null) continue;
            float maxHp = ally.HeroStatRuntime.MaxHealth;
            if (maxHp <= 0f) continue;

            float hp01 = ally.HeroStatRuntime.CurrentHealth / maxHp;
            if (hp01 < threshold)
                return true;
        }

        return false;
    }
    public Transform GetEnemyTransformWithLowestHpPercent()
    {
        if (BattlefieldRegistry.Instance == null)
            return null;

        string teamTag = CompareTag("Hero") ? "Enemy" : "Hero";
        var allies = BattlefieldRegistry.Instance.GetUnitsByTeam(teamTag);

        Transform best = null;
        float bestHp01 = float.PositiveInfinity;

        for (int i = 0; i < allies.Count; i++)
        {
            var root = allies[i];
            if (root == null) continue;

            var ally = root.GetComponent<HeroControl>();
            if (ally == null) continue;
            if (ally.LeftBattle) continue;

            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            if (ally.HeroStatRuntime == null) continue;
            float maxHp = ally.HeroStatRuntime.MaxHealth;
            if (maxHp <= 0f) continue;

            float hp01 = ally.HeroStatRuntime.CurrentHealth / maxHp;
            if (hp01 < bestHp01)
            {
                bestHp01 = hp01;
                best = root.transform;
            }
        }

        return best;
    }

    private void Awake()
    {
        normalT = transform.Find("Normal");
        receiveT = normalT.Find("ColliderReceive");
        fixedLayer = LayerMask.LayerToName(receiveT.gameObject.layer);
        animator = normalT.GetComponent<Animator>();
        spriteRenderer = normalT.GetComponent<SpriteRenderer>();
        Transform spriteEffectT = transform.Find("SpriteEffect");
        spriteEffect = spriteEffectT.GetComponent<SpriteRenderer>();
        SetUpZEffect();
        Transform listEffectT = transform.Find("ListEffect");
        listEffect = listEffectT;
        canvasTotalDamage = GameObject.Find("Canvas").GetComponent<UI_CanvasTotalDamage>();
        heroStatRuntime = GetComponent<HeroStatRuntime>();
        heroStateManager = GetComponent<HeroStateManager>();
        heroRun = GetComponent<HeroRun>();
        heroTransform = GetComponent<HeroTransform>();
        heroAttackk = GetComponent<HeroAttackk>();
        heroDodge = GetComponent<HeroDodge>();
        heroIdle = GetComponent<HeroIdle>();
        heroSkilll = GetComponent<HeroSkilll>();
        heroUltimate = GetComponent<HeroUltimate>();

        heroReceiveDamagee = transform.GetChild(0).Find("ColliderReceive").GetComponent<HeroReceiveDamagee>();
        capsuleCollider2D = heroReceiveDamagee.GetComponent<CapsuleCollider2D>();
        heroEventt = transform.GetChild(0).GetComponent<HeroEventt>();
    }
    void SetUpZEffect()
    {
        Vector3 pos = spriteEffect.transform.position;
        pos.z =  -0.01f;
        spriteEffect.transform.position = pos;
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
    public void RefreshObservers(string name, bool value)
    {
        NotifyObservers(name, value);
    }
    public void RefreshObservers(string value)
    {
        NotifyObservers( value);
    }

    public void RefreshObservers(object data1)
    {
        NotifyObservers(data1);
    }
    public void RefreshObservers(ModifyStatType type)
    {
        NotifyObservers(type);
    }
    public void RefreshObservers(ModifyStatType type, int value)
    {
        NotifyObservers(type, value);
    }
    public void RefreshObservers(AbilityEffectType type)
    {
        NotifyObservers(type);
    }
    public void RefreshObservers(HeroNotifyType type, object data = null)
    {
        NotifyObservers(type, data);
    }
    public void RefreshObservers(HPNotifyType type, object data = null)
    {
        NotifyObservers(type, data);
    }
    public void RefreshObservers(HPNotifyType type,  DamageType damageType, object data = null)
    {
        NotifyObservers(type,  damageType, data);
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
    public bool CheckEnemyDead(int enemyAmount)
    {
        int deadCount = 0;
        foreach (Transform enemy in enemyTarget)
        {
            if (enemy == null) continue;
            HeroReceiveDamagee recv = enemy.GetComponentInChildren<HeroReceiveDamagee>();
            if(recv != null && recv.IsDead)
            {
                deadCount++;
            }
        }
        if(deadCount >= enemyAmount)
        {
            return true;
        }
        else return false;
    }
    void LateUpdate()
    {
        spriteEffect.sprite = spriteRenderer.sprite;
        spriteEffect.flipX = spriteRenderer.flipX;
    }
}