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
        if (Input.GetKeyDown(KeyCode.I))
        {
            pc.Animator.SetTrigger("skill");
        }
    }
}
