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
        _NPC_StateMachine.OnStateChanged += SwitchAnimations;
        NPCAnimator = GetComponent<NPC_Animator>();

    }
    void Start()
    {
        
    }
    protected void Update()
    {
        _NPC_StateMachine.StateUpdate();
    }

    public virtual void Interact()
    {
        Debug.Log("Default Interaction");
    }

    private void SwitchAnimations() 
    {
        NPCAnimator.SwitchAnimationState("IdleVariant", Random.Range(0, 3));
    }


}
