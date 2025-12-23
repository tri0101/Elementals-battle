using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
   
    [SerializeField] private bool hasLeftGround;
    [SerializeField] private bool hasLanded;
    public override void EnterState(PlayerStateManager player)
    {
        hasLeftGround = false;
        Debug.Log("da goi jump");

        if (player.PlayerControl.HasBeenTransform)
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump);
        }
        else
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump);
        }
            
        player.PlayerControl.PlayerJump.Jump();

        player.PlayerControl.IsJumpPressed = false;
    }
    public override void ExitState(PlayerStateManager player)
    {
        
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if (!player.PlayerControl.PlayerCheckingGround.IsGrounded)
        {
            hasLeftGround = true;
        }


        if (player.PlayerControl.PlayerJump.CheckVelociyY() && hasLeftGround)
        {
            if (player.PlayerControl.HasBeenTransform)
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump_Down);
            }
            else
            {
                player.PlayerControl.ChangeAnimationStateLoop(PlayerStateManager.Player_Jump_Down);
            }
            
            player.PlayerControl.PlayerJump.SetGravity(7f);
        }
        if (player.PlayerControl.PlayerCheckingGround.IsGrounded && hasLeftGround)
        {
            hasLanded = true;

            if (player.PlayerControl.HasBeenTransform)
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump_End);
            }
            else
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump_End);
            }
               
            hasLeftGround = false;
        }

        Animator anim = player.PlayerControl.Animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (((info.IsName(PlayerStateManager.Player_Jump_End)|| info.IsName(PlayerStateManager.Player_T_Jump_End)) && info.normalizedTime >= 0.9f))
        {
            
            player.PlayerControl.PlayerJump.SetGravity(1f);
            hasLanded = false;
           player.SwitchState(player.idleState);
        }


    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (hasLanded)
        {
            player.PlayerControl.PlayerJump.MyFixedUpdate(0);
        }
        else
        {
            player.PlayerControl.PlayerJump.MyFixedUpdate(player.PlayerControl.MoveX);
        }
            
    }

    
}
