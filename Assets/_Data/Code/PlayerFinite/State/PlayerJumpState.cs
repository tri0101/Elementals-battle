using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Animator.SetTrigger("isJump");
        player.PlayerControl.PlayerJump.Jump();

        player.PlayerControl.IsJumpPressed = false;
    }
    public override void ExitState(PlayerStateManager player)
    {
        
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if (player.PlayerControl.PlayerCheckingGround.IsGrounded)
            player.SwitchState(player.idleState);
       
        

    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        player.PlayerControl.PlayerJump.UpdateAnimatorY();
    }
}
