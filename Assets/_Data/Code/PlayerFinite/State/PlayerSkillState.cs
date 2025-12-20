using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        player.PlayerControl.IsSkillPressed = false;
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Skill);
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
        AnimatorStateInfo info = player.PlayerControl.Animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 1f)
        {
            player.SwitchState(player.idleState);
        }
    }
}
