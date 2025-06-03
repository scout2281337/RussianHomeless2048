using UnityEngine;

public class PlayerCrouchingState : PlayerBaseState
{
    public PlayerCrouchingState(PlayerStates key, PlayerController context)
       : base(key, context)
    {
        _lerpAmount = 1f;
        _canAddBonusJumpApex = false;
    }

    public override void EnterState()
    {
        Context.SetGravityScale(Context.Data.gravityScale);
        Context.Crouch();
    }

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        Context.Run(_lerpAmount, _canAddBonusJumpApex);
    }

    public override void ExitState() { }

    public override PlayerStates GetNextState()
    {
        //if (!Context.IsGrounded)
        //{
        //    return PlayerStates.Falling;
        //}

        if (!Context.IsBlockedUp)
        {
            if (Context.JumpRequest || Context.CrouchRequest)
            {
                Context.UnCrouch();
                return PlayerStates.Grounded;
            }
        }

        return StateKey;
    }
}
