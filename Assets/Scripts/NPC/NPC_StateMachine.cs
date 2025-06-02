

public class NPC_StateMachine
{
    private IState currentState;

    public void SetState(IState newState) 
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();  
    }
    
    public void StateUpdate()
    {
        currentState?.Update();   
    }
}
