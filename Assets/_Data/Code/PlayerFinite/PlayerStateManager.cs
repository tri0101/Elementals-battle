using UnityEngine;
using System.Collections.Generic;
public class PlayerStateManager : MonoBehaviour
{



    [Header("PlayerController")]
    PlayerControl playerControl;
    public PlayerControl PlayerControl => playerControl;


    [Header ("State")]
    PlayerBaseState currentState;
    public PlayerIdleState idleState = new PlayerIdleState();
    public PlayerRunState runState = new PlayerRunState();
    public PlayerJumpState jumpState = new PlayerJumpState();
    public PlayerAttackState attackState = new PlayerAttackState();
    public PlayerSkillState skillState = new PlayerSkillState();
    public PlayerBlockState blocKState = new PlayerBlockState();
    public PlayerTransformState transformState = new PlayerTransformState();

    //List các state normal
    public const string Player_Idle = "Idle";
    public const string Player_Run = "Run";
    public const string Player_Jump = "Jump Start";
    public const string Player_Jump_Down = "Jump Down";
    public const string Player_Jump_End = "Jump End";
    public const string Player_Attack_1 = "Attack_1";
    public const string Player_Attack_2 = "Attack_2";
    public const string Player_Attack_3 = "Attack_3";
    public const string Player_Skill = "Skill";
    public const string Player_Block_Open = "Block_open";
    public const string Player_Block = "Block";
    public const string Player_Block_End = "Block_End";
    public const string Player_Transform = "Transform";
    //List các state transform
    public const string Player_T_Idle = "T_Idle";
    public const string Player_T_Run = "T_Run";
    public const string Player_T_Jump = "T_Jump_Start";
    public const string Player_T_Jump_Down = "T_Jump_Down";
    public const string Player_T_Jump_End = "T_Jump_End";
    public const string Player_T_Attack_1 = "T_Attack_1";
    public const string Player_T_Attack_2 = "T_Attack_2";
    public const string Player_T_Attack_3 = "T_Attack_3";
    public const string Player_Transform_To_Human = "TransformToHuman";
    //public const string Player_Skill = "Skill";
    //public const string Player_Block_Open = "Block_open";
    //public const string Player_Block = "Block";
    //public const string Player_Block_End = "Block_End";
    //public const string Player_Transform = "Transform";

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }

    private void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }
    private void Update()
    {
        currentState.UpdateState(this);
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }
    //public void SwitchState(PlayerBaseState state)
    //{
    //    currentState = state;
    //    state.EnterState(this);
    //}

    public void SwitchState(PlayerBaseState newState)
    {
        if (currentState == newState) return;

        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

   

}
