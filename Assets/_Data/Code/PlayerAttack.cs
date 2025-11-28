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
        if (Input.GetKeyDown(pc.KeyBiding.attackKey) && CanAttack)
        {
            Attack();
        }
    }
    private void Attack()
    {
        pc.Animator.SetTrigger("attack");
    }
}
