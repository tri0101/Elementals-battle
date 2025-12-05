using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerController pc;
    public bool CanAttack
    {
        get => pc.Animator.GetBool("canAttack");
        set => pc.Animator.SetBool("canAttack", value);
    }
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(pc.KeyBiding.attackKey) && CanAttack && !pc.PlayerReceiveDamage.IsHit)
        {
            Attack();
        }
        else if(Input.GetKeyDown(pc.KeyBiding.rangedAttackKey) && CanAttack && !pc.PlayerReceiveDamage.IsHit)
        {
            
            RangedAttack();
            
        }
    }
    private void Attack()
    {
        pc.Animator.SetTrigger("attack");
    }
    private void RangedAttack()
    {
        pc.Animator.SetTrigger("rangedAttack");
       
    }
}
