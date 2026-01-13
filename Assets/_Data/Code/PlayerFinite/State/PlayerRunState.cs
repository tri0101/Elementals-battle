using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        //if (player.PlayerControl.IsSkillOnePressed)
        //{
        //    player.SwitchState(player.skillOneState);
        //    return;
        //}
       
        if (player.PlayerControl.MoveX == 0)
            player.SwitchState(player.idleState);

        player.PlayerControl.PlayerRun.Flip();
    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        player.PlayerControl.PlayerRun.Move();
    }
    public override void LateUpdateState(PlayerStateManager player)
    {

    }
}
