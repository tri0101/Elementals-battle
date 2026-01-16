using UnityEngine;

public abstract class HeroBaseState
{
    public abstract void EnterState(HeroStateManager hero);
    public abstract void UpdateState(HeroStateManager hero);
    public abstract void FixedUpdateState(HeroStateManager hero);
    public abstract void LateUpdateState(HeroStateManager hero);
    public abstract void ExitState(HeroStateManager hero);




}
