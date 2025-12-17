using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{

    public override void EnterState(PlayerStateManager player)
    {
        //player.PlayerControl.Animator.SetBool("isWalking", false);
        player.PlayerControl.ChangeAnimationState(player.Player_Idle);
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
    }
    public override void ExitState(PlayerStateManager player)
    {
        
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if (player.PlayerControl.IsJumpPressed && player.PlayerControl.PlayerCheckingGround.IsGrounded)
        {
            player.SwitchState(player.jumpState);
        }
        if (player.PlayerControl.MoveX != 0)
        {
            player.SwitchState(player.runState);
        }
    }
    public override void FixedUpdateState(PlayerStateManager player)
    {

    }
}
