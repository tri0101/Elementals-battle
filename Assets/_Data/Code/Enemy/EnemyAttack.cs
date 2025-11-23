//using UnityEngine;

//public class EnemyAttack : MonoBehaviour
//{

//    public float attackRange = 5f; // khoảng cách để tấn công
//    public float attackCooldown = 0.3f; // cooldown giữa các đòn
//    private float lastAttackTime = 0f;

//    private EnemyController ec;    // giống PlayerController cho Enemy
//    private PlayerController pc;
//    public bool CanAttack
//    {
//        get => ec.Animator.GetBool("canAttack");
//        set => ec.Animator.SetBool("canAttack", value);
//    }

//    private void Awake()
//    {
//        ec = GetComponent<EnemyController>();

//    }
//    private void Start()
//    {
//        pc = ec.Player.GetComponent<PlayerController>();
//    }
//    private void Update()
//    {
//        if (ec.Player == null|| !pc.PlayerReceiveDamage.IsAlive) return;

//        float distance = Vector2.Distance(transform.position, ec.Player.position);

//        if (distance <= attackRange && CanAttack && Time.time >= lastAttackTime + attackCooldown)
//        {
//            Attack();
//        }
//    }

//    private void Attack()
//    {
//        ec.Animator.SetTrigger("attack");
//        lastAttackTime = Time.time;
//    }
//}
using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 5f;
    public float attackCooldown = 0.25f;
    public float longCooldown = 1.5f;
    public int maxCombo = 4;

    private int attackCount = 0;
    private bool isResting = false;
    private float lastAttackTime = 0f;

    private EnemyController ec;
    private PlayerController pc;

    public bool CanAttack
    {
        get => ec.Animator.GetBool("canAttack");
        set => ec.Animator.SetBool("canAttack", value);
    }

    private void Awake()
    {
        ec = GetComponent<EnemyController>();
    }

    private void Start()
    {
        if (ec.Player != null)
            pc = ec.Player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (ec.Player == null || pc == null || !pc.PlayerReceiveDamage.IsAlive) return;

        float distance = Vector2.Distance(transform.position, ec.Player.position);

        if (distance <= attackRange && CanAttack && !isResting)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        ec.Animator.SetTrigger("attack");
        lastAttackTime = Time.time;
        attackCount++;

        if (attackCount >= maxCombo)
        {
            // Bắt đầu nghỉ dài
            StartCoroutine(RestAfterCombo());
        }
    }

    private IEnumerator RestAfterCombo()
    {
        isResting = true;
        yield return new WaitForSeconds(longCooldown);
        attackCount = 0;
        isResting = false;
    }
}
