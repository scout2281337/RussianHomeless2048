using UnityEngine;
using UnityEngine.AI;


public class NPC_Brain : MonoBehaviour
{
    public NPC_StateMachine _NPC_StateMachine;
    public NavMeshAgent Agent;
    public Transform point;
    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        _NPC_StateMachine = new NPC_StateMachine();
        
        _NPC_StateMachine.SetState(new PatrolState(this));

    }
    void Start()
    {
        
    }
    protected void Update()
    {
        _NPC_StateMachine.StateUpdate();    
    }


}
