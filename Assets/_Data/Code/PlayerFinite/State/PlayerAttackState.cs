using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    bool canCombo;

    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        player.PlayerControl.IsAttackPressed = false;
        player.PlayerControl.PlayerAttackk.CountAttack = 0;
        PlayAttack(player);
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        AnimatorStateInfo info = player.PlayerControl.Animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.8f)
            canCombo = true;
        if (canCombo && player.PlayerControl.IsAttackPressed)
        {
            player.PlayerControl.IsAttackPressed = false;
            player.PlayerControl.PlayerAttackk.CountAttack++;

            if (player.PlayerControl.PlayerAttackk.CountAttack <= 2)
            {
                PlayAttack(player);
                return;
            }
        }

        if (info.normalizedTime >= 1f)
        {
            player.SwitchState(player.idleState);
        }
    }

    void PlayAttack(PlayerStateManager player)
    {
        canCombo = false;

        if (player.PlayerControl.HasBeenTransform)
        {
            switch (player.PlayerControl.PlayerAttackk.CountAttack)
            {
                case 0:
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Attack_1);
                    break;
                case 1:
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Attack_2);
                    break;
                case 2:
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Attack_3);
                    break;
            }
        }
        else
        {
            switch (player.PlayerControl.PlayerAttackk.CountAttack)
            {
                case 0:
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Attack_1);
                    break;
                case 1:
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Attack_2);
                    break;
                case 2:
                    player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Attack_3);
                    break;
            }
        }
       
    }
    public override void LateUpdateState(PlayerStateManager player)
    {

    }
}
