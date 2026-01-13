using TMPro;
using UnityEngine;

public class PlayerTakeHitState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {

        
            player.PlayerControl.ChangeAnimationAnyState(PlayerStateManager.Player_Take_Hit);
        
            
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
        if (player.PlayerControl.PlayerReceiveDamagee.IsDead)
        {
            player.SwitchState(player.deathState);
            return;
        }
        if (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Take_Hit, 0.99f, 1) || player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Take_Hit, 0.99f, 1))
        {

            
            
                Debug.Log("ve idle");
                player.SwitchState(player.idleState);
            

        }
      


    }
}
