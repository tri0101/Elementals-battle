using UnityEngine;

public class PlayerTakeHitState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.PlayerReceiveDamagee.IsHit = false;
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Take_Hit);
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
       
    }

    public override void LateUpdateState(PlayerStateManager player)
    {
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Take_Hit, 0.95f,1))
        {
            if (player.PlayerControl.WasInAirBeforeHit)
            {
                player.SwitchState(player.jumpState);
            }
            else
            {
                player.SwitchState(player.idleState);
            }
                
        }
    }
}
