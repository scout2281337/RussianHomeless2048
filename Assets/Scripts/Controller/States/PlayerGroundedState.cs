using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStates key, PlayerController context)
        : base(key, context)
    {
        _lerpAmount = 1f;
        _canAddBonusJumpApex = false;
    }

    public override void EnterState()
    {
        Context.SetGravityScale(Context.Data.gravityScale);
    }

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        Context.Run(_lerpAmount, _canAddBonusJumpApex);
        Context.SetCameraPosition(Mathf.Abs(Context.Velocity.x) > .05f || Mathf.Abs(Context.Velocity.z) > .05f ? CameraPosition.run : CameraPosition.def);
    }

    public override void ExitState() { }

    public override PlayerStates GetNextState()
    {
        //set coyote time just when falling
        if (!Context.IsGrounded)
        {
            Context.IsActiveCoyoteTime = true;
            return PlayerStates.Falling;
        }

        if (Context.JumpRequest)
        {
            Context.IsActiveCoyoteTime = false;
            return PlayerStates.Jumping;
        }

        if (Context.CrouchRequest)
        {
            return PlayerStates.Crouching;
        }

        if (Context.IsFighting)
        {
            return PlayerStates.FightIdle;
        }
        return StateKey;
    }
}
