using UnityEngine;

public class OnFinish : StateMachineBehaviour
{
    [SerializeField] private string animation;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<PlayerControl>().ChangeAnimationState(animation, stateInfo.length);
    }
}
