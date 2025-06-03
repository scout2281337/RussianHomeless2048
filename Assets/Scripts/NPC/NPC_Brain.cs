using UnityEngine;
using UnityEngine.AI;


public class NPC_Brain : MonoBehaviour, IInteractable
{
    [HideInInspector]public NPC_StateMachine _NPC_StateMachine;
    [HideInInspector]public NPC_Animator NPCAnimator { get; private set; }

    public NavMeshAgent Agent;
    public Transform point;
    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        _NPC_StateMachine = new NPC_StateMachine();
        _NPC_StateMachine.SetState(new PatrolState(this));

        NPCAnimator = GetComponent<NPC_Animator>();

    }
    void Start()
    {
        
    }
    protected void Update()
    {
        _NPC_StateMachine.StateUpdate();
        float speed = Agent.velocity.magnitude;
        if ( NPCAnimator != null ) 
        {
            NPCAnimator.SetSpeed(speed);
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Default Interaction");
    }


}
