using UnityEngine;

public class PlayerRangedAttackState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        player.PlayerControl.IsRangedAttackPressed = false;
        player.PlayerControl.RefreshObservers((PlayerControl.RangedAttackObserver, player.PlayerControl.PlayerInfo.durationRangedAttack));
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Ranged_Attack);

    }

    public override void ExitState(PlayerStateManager player)
    {
       
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Ranged_Attack, 0.9f, 1)))
        {

            player.SwitchState(player.idleState);
        }
    }
    public override void LateUpdateState(PlayerStateManager player)
    {

    }
}
