using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    private float _timeInState;

    public PlayerFallingState(PlayerStates key, PlayerController context)
        : base(key, context)
    {
        _lerpAmount = 1f;
        _canAddBonusJumpApex = true;
    }

    public override void EnterState()
    {
        _timeInState = 0f;
    }

    public override void UpdateState()
    {
        // coyote time
        if (_timeInState <= Context.Data.coyoteTime)
        {
            _timeInState += Time.deltaTime;
        }
        else
        {
            Context.IsActiveCoyoteTime = false;
        }

        float gravityScale = Context.Data.gravityScale;
        gravityScale *= Context.Data.fallGravityMult;

        Context.SetGravityScale(gravityScale);
    }

    public override void FixedUpdateState()
    {
        // limit vertical velocity
        float terminalVelocity = -Context.Data.maxFallSpeed;
        // higher fall velocity if holding down
        if (Context.MovementDirection.y < 0)
            terminalVelocity = -Context.Data.maxFastFallSpeed;

        Context.Velocity = new Vector3(Context.Velocity.x, Mathf.Max(Context.Velocity.y, terminalVelocity), Context.Velocity.z);

        Context.Run(_lerpAmount, _canAddBonusJumpApex);
    }

    public override void ExitState()
    {
        Context.IsActiveCoyoteTime = false;
    }

    public override PlayerStates GetNextState()
    {
        if (Context.IsGrounded)
            return PlayerStates.Grounded;

        if (Context.JumpRequest)
        {
            if (Context.IsActiveCoyoteTime)
                return PlayerStates.Jumping;
        }

        return StateKey;
    }
}
