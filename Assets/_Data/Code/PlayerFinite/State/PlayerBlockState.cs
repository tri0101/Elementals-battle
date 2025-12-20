using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    bool isEnding;
    public override void EnterState(PlayerStateManager player)
    {
        isEnding = false;
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Block_Open);
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.PlayerControl.IsBlockPressed = false;
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (!player.PlayerControl.IsBlockPressed && !isEnding)
        {
            isEnding = true;
            player.PlayerControl.IsBlockPressed = false;
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Block_End);
        }
        if (!isEnding) return;
        Animator anim = player.PlayerControl.Animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName(PlayerStateManager.Player_Block_End) && info.normalizedTime >= 0.9f)
        {
      

            player.SwitchState(player.idleState);
        }
    }

  
}
