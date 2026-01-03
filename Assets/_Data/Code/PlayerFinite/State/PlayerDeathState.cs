using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.ChangeLayerAtReceive("RollLayer");
        player.PlayerControl.ChangeAnimationAnyState(PlayerStateManager.Player_Dead);
        
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
    }
}
