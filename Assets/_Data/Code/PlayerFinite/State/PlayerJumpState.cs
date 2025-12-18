using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
   
    [SerializeField] private bool hasLeftGround;
    public override void EnterState(PlayerStateManager player)
    {
        hasLeftGround = false;
        Debug.Log("da goi jump");
        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump);
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
            player.PlayerControl.ChangeAnimationStateLoop(PlayerStateManager.Player_Jump_Down);
            player.PlayerControl.PlayerJump.SetGravity(7f);
        }
        if (player.PlayerControl.PlayerCheckingGround.IsGrounded && hasLeftGround)
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump_End);
            hasLeftGround = false;
        }

        Animator anim = player.PlayerControl.Animator;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName(PlayerStateManager.Player_Jump_End) && info.normalizedTime >= 1f)
        {
            player.PlayerControl.PlayerJump.SetGravity(1f);

           player.SwitchState(player.idleState);
        }


    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        player.PlayerControl.PlayerJump.MyFixedUpdate();
    }

    
}
