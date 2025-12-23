using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    bool isEnding;
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.IsBlockPressed = true;
        isEnding = false;
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        if (player.PlayerControl.HasBeenTransform)
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Block_Open);
        }
        else
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Block_Open);
        }
            
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {
      
        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Block_Open, 1f, 0) || (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Block_Open, 1, 0))))
        {

            return;
        }
        if (player.PlayerControl.PlayerTransformm.TransformToHuman)
        {
            isEnding = true;
            player.PlayerControl.IsBlockPressed = false;
            player.SwitchState(player.transformState);
            
            return;
        }
        if (!player.PlayerControl.IsBlockPressed && !isEnding)
        {
            isEnding = true;
            player.PlayerControl.IsBlockPressed = false;
            if(player.PlayerControl.HasBeenTransform)
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Block_End);
            }
            else
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Block_End);
            }
                
        }
        if (!isEnding) return;
        
        
        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Block_End, 0.9f, 1) || (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Block_End, 0.9f, 1))))
        {

            
            player.SwitchState(player.idleState);
        }
    }

  
}
