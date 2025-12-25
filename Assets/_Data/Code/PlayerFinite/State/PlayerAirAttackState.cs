using UnityEngine;

public class PlayerAirAttackState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.IsAttackPressed = false;
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Air_Attack);
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
    }


}
