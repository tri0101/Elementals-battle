using Unity.VisualScripting;
using UnityEngine;

public class PlayerTransformState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        if (player.PlayerControl.PlayerTransformm.TransformToHuman)
        {
           
            //player.PlayerControl.HasBeenTransform = false;
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Transform_To_Human);
            player.PlayerControl.PlayerTransformm.TransformToHuman = false;
            return;

        }
        player.PlayerControl.IsTransformPressed = false;
        

        
        
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Transform);
        
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        
    }

    public override void UpdateState(PlayerStateManager player)
    {

       
        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Transform, 0.9f, 1)))
        {
            player.PlayerControl.HasBeenTransform = true;
            player.PlayerControl.PlayerTransformm.ResetMana();
            player.SwitchState(player.idleState);
        }
        else if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Transform_To_Human, 0.9f, 1)))
        {
            player.PlayerControl.HasBeenTransform = false;
           
            player.SwitchState(player.idleState);
        }
    }
    public override void LateUpdateState(PlayerStateManager player)
    {

    }
}
