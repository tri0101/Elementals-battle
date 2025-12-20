using UnityEngine;

public class PlayerRunState : PlayerBaseState
{

   
    public override void EnterState(PlayerStateManager player)
    {
     
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Run);
     
    }
    public override void ExitState(PlayerStateManager player)
    {
       
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if(player.PlayerControl.IsJumpPressed && player.PlayerControl.PlayerCheckingGround.IsGrounded)
        {
            player.SwitchState(player.jumpState);
            return;
        }
        if (player.PlayerControl.IsAttackPressed)
        {
            player.SwitchState(player.attackState);
            return;
        }
        if (player.PlayerControl.IsSkillPressed)
        {
            player.SwitchState(player.skillState);
            return;
        }
        if (player.PlayerControl.IsBlockPressed)
        {
            player.SwitchState(player.blocKState);
            return;
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
