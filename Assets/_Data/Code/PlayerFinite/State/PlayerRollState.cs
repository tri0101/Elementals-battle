//using UnityEngine;

//public class PlayerRollState : PlayerBaseState
//{
//    public override void EnterState(PlayerStateManager player)
//    {
//        player.PlayerControl.IsRollPressed = false;
//        player.PlayerControl.ChangeLayerAtReceive("RollLayer");
//        player.PlayerControl.Rb.linearVelocity = new Vector2(0, player.PlayerControl.Rb.linearVelocity.y);
//        player.PlayerControl.ChangeAnimationState(PlayerStateManager.Player_Roll);
//        player.PlayerControl.PlayerRoll.StartRoll();
//    }

//    public override void ExitState(PlayerStateManager player)
//    {
       
//    }

//    public override void FixedUpdateState(PlayerStateManager player)
//    {
        
//    }

//    public override void UpdateState(PlayerStateManager player)
//    {

        
//        if ((player.PlayerControl.CheckCurrentAnimation(PlayerStateManager.Player_Roll, 0.95f, 1)))
//        {

//            player.PlayerControl.ReturnFixedLayer();
//            player.SwitchState(player.idleState);
//        }
//    }
//    public override void LateUpdateState(PlayerStateManager player)
//    {

//    }
//}
