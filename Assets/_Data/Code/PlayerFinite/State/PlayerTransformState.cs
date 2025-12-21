using Unity.VisualScripting;
using UnityEngine;

public class PlayerTransformState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        if (player.PlayerControl.PlayerTransformm.TransformToHuman)
        {
           
            player.PlayerControl.HasBeenTransform = false;
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Transform_To_Human);
            player.PlayerControl.PlayerTransformm.TransformToHuman = false;
            return;

        }
        player.PlayerControl.IsTransformPressed = false;
        player.PlayerControl.HasBeenTransform = true;

        
        
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

        Animator anim = player.PlayerControl.Animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if ((info.IsName(PlayerStateManager.Player_Transform) || info.IsName(PlayerStateManager.Player_Transform_To_Human)) && info.normalizedTime >= 0.9f)
        {


            player.SwitchState(player.idleState);
        }
    }
}
