//using UnityEngine;

//public class PlayerSkillOneState : PlayerBaseState
//{
//    public override void EnterState(PlayerStateManager player)
//    {
//        if(player.PlayerControl.transform.name == "Fire_Knight_finite"   && !player.PlayerControl.HasBeenTransform)
//        {

//        }
//        else
//        {
//            player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
//        }
//            player.PlayerControl.IsSkillOnePressed = false;

//        if (player.PlayerControl.HasBeenTransform)
//        {
//            player.PlayerControl.RefreshObservers((PlayerControl.SkillOneObserver, player.PlayerControl.PlayerInfo.durationTransformSkill1));
//        }
//        else
//        {
//            player.PlayerControl.RefreshObservers((PlayerControl.SkillOneObserver, player.PlayerControl.PlayerInfo.durationSkillOne));
//        }

//        if (player.PlayerControl.HasBeenTransform)
//        {
//            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_T_Skill_One);
//        }
//        else
//        {
//            player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Skill_One);
//        }

//    }

//    public override void ExitState(PlayerStateManager player)
//    {

//    }

//    public override void FixedUpdateState(PlayerStateManager player)
//    {
//        if (player.PlayerControl.transform.name == "Fire_Knight_finite" && !player.PlayerControl.HasBeenTransform)
//        {
//            player.PlayerControl.PlayerSkillOne.Move();
//        }
        
//    }

//    public override void UpdateState(PlayerStateManager player)
//    {
        
//        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Skill_One, 0.95f, 1) || (player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_T_Skill_One, 0.95f, 1))))
//        {
            
//            //player.PlayerControl.PlayerSkilll.ResetMana();

//            player.SwitchState(player.idleState);
//        }

//    }
//    public override void LateUpdateState(PlayerStateManager player)
//    {

//    }
//}
