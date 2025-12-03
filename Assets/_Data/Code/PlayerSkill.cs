using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    PlayerController pc;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(pc.KeyBiding.skillKey) && pc.Animator.GetFloat("yVelocity") == 0)
        {
            pc.Animator.SetTrigger("skill");
        }
    }
}
