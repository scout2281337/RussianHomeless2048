

using System;

public class NPC_StateMachine
{
    private IState currentState;

    public event Action OnStateChanged;

    public void SetState(IState newState) 
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();

        OnStateChanged?.Invoke();
    }
    
    public void StateUpdate()
    {
        currentState?.Update();   
    }
}
