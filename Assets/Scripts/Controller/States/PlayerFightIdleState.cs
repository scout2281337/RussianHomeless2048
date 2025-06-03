using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerFightIdleState : PlayerBaseState
{
    public PlayerFightIdleState(PlayerStates key, PlayerController context)
        : base(key, context)
    {
        _lerpAmount = 1f;
        _canAddBonusJumpApex = false;
    }

    public override void EnterState()
    {
        Context.SetGravityScale(Context.Data.gravityScale);
        Context.SetCameraPosition(CameraPosition.fight);
        Context.SetHandsIK(true);
    }

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        Context.Run(_lerpAmount, _canAddBonusJumpApex);
    }

    public override void ExitState() { }

    public override PlayerStates GetNextState()
    {
        //set coyote time just when falling
        if (!Context.IsGrounded)
        {
            Context.SetHandsIK(false);
            Context.IsActiveCoyoteTime = true;
            return PlayerStates.Falling;
        }

        if (Context.IsGrounded && !Context.IsFighting)// || Context.IsMovingHorizontal)
        {
            Context.SetHandsIK(false);
            return PlayerStates.Grounded;
        }

        if (Context.JumpRequest)
        {
            Context.SetHandsIK(false);
            Context.IsActiveCoyoteTime = false;
            return PlayerStates.Jumping;
        }

        if (Context.CrouchRequest)
        {
            Context.SetHandsIK(false);
            return PlayerStates.Crouching;
        }

        if (Context.PunchRequest && Context.CanPunch)
        {
            return PlayerStates.FightPunch;
        }
        return StateKey;
    }
}
