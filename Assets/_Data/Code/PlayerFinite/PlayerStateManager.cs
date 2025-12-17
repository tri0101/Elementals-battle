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

    //List các state
    public string Player_Idle = "Idle";
    public string Player_Run = "Run";
    public string Player_Jump = "jump_rising";


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
