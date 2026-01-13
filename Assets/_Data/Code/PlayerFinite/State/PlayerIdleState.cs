using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerIdleState : PlayerBaseState
{

    public override void EnterState(PlayerStateManager player)
    {
        
       
        player.PlayerControl.ChangeAnimationState((PlayerStateManager.Player_Idle));
        
           

        //Để nhân vật ko bị trượt
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
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
        
        if (player.PlayerControl.MoveX != 0)
        {
            player.SwitchState(player.runState);
        }

    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(player.PlayerControl.MoveX, player.PlayerControl.Rb.linearVelocity.y);
    }

    public override void LateUpdateState(PlayerStateManager player)
    {
        
    }
}
