using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
   
    [SerializeField] private bool hasLeftGround;
    [SerializeField] private bool hasLanded;

    [SerializeField] private bool isAttacking;
    //public override void EnterState(PlayerStateManager player)
    //{
    //    hasLeftGround = false;
    //    isAttacking = false;

    //    Debug.Log("da goi jump");

    //    if (player.PlayerControl.HasBeenTransform)
    //    {
    //        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump);
    //    }
    //    else
    //    {
    //        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump);
    //    }

    //    player.PlayerControl.PlayerJump.Jump();

    //    player.PlayerControl.IsJumpPressed = false;
    //}
    public override void EnterState(PlayerStateManager player)
    {
        hasLeftGround = !player.PlayerControl.PlayerCheckingGround.IsGrounded;
        isAttacking = false;

        // Nếu từ TakeHit quay lại mà đang trên không → KHÔNG Jump lại
        if (!hasLeftGround)
        {
            if (player.PlayerControl.HasBeenTransform)
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump);
            else
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump);

            player.PlayerControl.PlayerJump.Jump();
        }
        else
        {
            // Resume đúng phase
            if (player.PlayerControl.WasFallingBeforeHit)
            {
                if (player.PlayerControl.HasBeenTransform)
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump_Down);
                else
                    player.PlayerControl.ChangeAnimationStateLoop(PlayerStateManager.Player_Jump_Down);
            }
        }

        player.PlayerControl.IsJumpPressed = false;
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.PlayerControl.HasAttackedWhenJump = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {

        
        if (player.PlayerControl.IsAttackPressed && !player.PlayerControl.HasAttackedWhenJump)
        {
            isAttacking = true;
            player.PlayerControl.HasAttackedWhenJump = true;
            player.PlayerControl.IsAttackPressed = false;
            if (player.PlayerControl.HasBeenTransform)
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Air_Attack);
            }
            else
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Air_Attack);
            }
                
            return;
        }
        if (isAttacking)
        {
            if (!player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Air_Attack, 0.95f, 1) &&
                !player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Air_Attack,0.95f,1))
            {
                return;
            }
            isAttacking = false;
        }
        if (player.PlayerControl.HasBeenTransform)
        {
            if (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Jump, 0.95f, 1))
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Jump_Rising);
            }
        }
        else
        {
            if (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Jump, 0.95f, 1))
            {
                player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Jump_ring);
            }
        }
        
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

        

        
        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Jump_End, 0.9f, 1) || (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Jump_End, 0.9f,1 ))))
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
        player.PlayerControl.PlayerJump.CheckPlayer();
    }

    public override void LateUpdateState(PlayerStateManager player)
    {

    }
}
