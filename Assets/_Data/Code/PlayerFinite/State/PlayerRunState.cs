using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerRunState : PlayerBaseState
{

   
    public override void EnterState(PlayerStateManager player)
    {
        if (player.PlayerControl.HasBeenTransform)
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Run);

        }
        else
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Run);
        }
        
     
    }
    public override void ExitState(PlayerStateManager player)
    {
       
    }
    public override void UpdateState(PlayerStateManager player)
    {


        if (player.PlayerControl.IsRollPressed)
        {
            player.SwitchState(player.rollState);
        }
        if (player.PlayerControl.PlayerTransformm.TransformToHuman)
        {
            player.SwitchState(player.transformState);

            return;
        }
        if (player.PlayerControl.IsJumpPressed && player.PlayerControl.PlayerCheckingGround.IsGrounded)
        {
            player.SwitchState(player.jumpState);
            return;
        }
        if (player.PlayerControl.IsAttackPressed)
        {
            player.SwitchState(player.attackState);
            return;
        }
        if (player.PlayerControl.IsRangedAttackPressed)
        {
            player.SwitchState(player.rangedAttackState);
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
        if (player.PlayerControl.IsTransformPressed)
        {
            player.SwitchState(player.transformState);
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
