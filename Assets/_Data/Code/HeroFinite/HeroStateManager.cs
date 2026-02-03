using UnityEngine;
using System.Collections.Generic;
public class HeroStateManager : MonoBehaviour
{



    [Header("heroController")]
    HeroControl heroControl;
    public HeroControl HeroControl => heroControl;


    [Header ("State")]
    HeroBaseState currentState;
    public HeroIdleState idleState = new HeroIdleState();
    public HeroRunState runState = new HeroRunState();

    public HeroAttackState attackState = new HeroAttackState();

  
    public HeroSkillState skillState = new HeroSkillState();

   
    public HeroTakeHitState takeHitState = new HeroTakeHitState();
    public HeroDeathState deathState = new HeroDeathState();

    //List các state normal
    public const string hero_Idle = "Idle";
    public const string hero_Run = "Run";
    public const string hero_Attack_1 = "Attack_1";
    public const string hero_Attack_2 = "Attack_2";
    public const string hero_Skill = "Skill";
    public const string hero_Take_Hit = "Take_hit";
    public const string hero_Dead = "death";
    //List các state transform
 


    private void Awake()
    {
        heroControl = GetComponent<HeroControl>();
    }

    private void Start()
    {
        //currentState = idleState;
        currentState = runState;
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
    private void LateUpdate()
    {
        currentState.LateUpdateState(this);
    }

    public void SwitchState(HeroBaseState newState)
    {
        if (currentState == newState) return;

        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

   
    public void SwitchAnyState(HeroBaseState newState)
    {
        

        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

   

}
