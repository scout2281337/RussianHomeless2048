using UnityEngine;

public class PlayerPunchState : PlayerBaseState
{
    private float _timeInState;
    public PlayerPunchState(PlayerStates key, PlayerController context)
        : base(key, context)
    {
        _lerpAmount = 1f;
        _canAddBonusJumpApex = true;
    }

    public override void EnterState()
    {
        _timeInState = 0f;
        Context.SetGravityScale(Context.Data.gravityScale);
        Context.IsPunchActive = false;
        Context.PunchAnimRequest = true;
        Context.Punch();
    }

    public override void UpdateState()
    {
        _timeInState += Time.deltaTime;
    }

    public override void FixedUpdateState()
    {
        Context.Run(_lerpAmount, _canAddBonusJumpApex); //?
    }

    public override void ExitState() 
    {
        Context.RefillPunch();
        Context.IsPunchActive = true;
    }

    public override PlayerStates GetNextState()
    {
        if (_timeInState >= Context.Data.punchTime)
        {
            return PlayerStates.FightIdle;
        }

        return StateKey;
    }
}
