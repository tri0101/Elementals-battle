using UnityEngine;

public class PlayerRunState : PlayerBaseState
{

   
    public override void EnterState(PlayerStateManager player)
    {
        //player.PlayerControl.Animator.SetBool("isWalking", true);
        player.PlayerControl.ChangeAnimationState(player.Player_Run);
     
    }
    public override void ExitState(PlayerStateManager player)
    {
       
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if(player.PlayerControl.IsJumpPressed && player.PlayerControl.PlayerCheckingGround.IsGrounded)
        {
            player.SwitchState(player.jumpState);
        }
        if (player.PlayerControl.MoveX == 0)
            player.SwitchState(player.idleState);

        player.PlayerControl.PlayerRun.Flip();
    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        player.PlayerControl.PlayerRun.Move();
    }
}
