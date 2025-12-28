using UnityEngine;

public class PlayerTakeHitState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.PlayerReceiveDamagee.IsHit = false;
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Take_Hit);
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
            player.SwitchState(player.idleState);
        }
    }
}
