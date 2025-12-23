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
        Animator anim = player.PlayerControl.Animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if ((info.IsName(PlayerStateManager.Player_Block_Open) || info.IsName(PlayerStateManager.Player_T_Block_Open)) && info.normalizedTime <= 1f)
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
        //Animator anim = player.PlayerControl.Animator;

        //AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if ((info.IsName(PlayerStateManager.Player_Block_End) || info.IsName(PlayerStateManager.Player_T_Block_End)) && info.normalizedTime >= 0.9f)
        {

            
            player.SwitchState(player.idleState);
        }
    }

  
}
