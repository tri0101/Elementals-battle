using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
        player.PlayerControl.IsSkillPressed = false;

        player.PlayerControl.RefreshObservers((PlayerControl.SkillObserver, player.PlayerControl.PlayerInfo.durationSkill));
        if (player.PlayerControl.HasBeenTransform)
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Skill);
        }
        else
        {
            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Skill);
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
       
        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Skill, 0.9f, 1)|| (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Skill, 0.9f, 1))))
        {

            //player.PlayerControl.PlayerSkilll.ResetMana();
           
            player.SwitchState(player.idleState);
        }

    }
    public override void LateUpdateState(PlayerStateManager player)
    {

    }
}
