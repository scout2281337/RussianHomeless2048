using UnityEngine;


public class NPC_Brain : MonoBehaviour
{
    public NPC_StateMachine _NPC_StateMachine;
    void Awake()
    {
        _NPC_StateMachine = new NPC_StateMachine();
        _NPC_StateMachine.SetState(new IdleState(this));   
    }
    void Start()
    {
        
    }
    void Update()
    {
        _NPC_StateMachine.StateUpdate();    
    }


}
