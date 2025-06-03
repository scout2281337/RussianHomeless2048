using UnityEngine;



public abstract class PlayerBaseState : BaseState<PlayerStates>
{
    protected float _lerpAmount;
    protected bool _canAddBonusJumpApex;

    public PlayerController Context { get; private set; }

    protected PlayerBaseState(PlayerStates key, PlayerController context)
        : base(key)
    {
        Context = context;
    }
}
public enum PlayerStates
{
    Grounded, Jumping, Falling, Crouching, FightIdle, FightPunch
}
