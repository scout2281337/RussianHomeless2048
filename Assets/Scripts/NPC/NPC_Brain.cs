using UnityEngine;
using UnityEngine.AI;


public class NPC_Brain : MonoBehaviour, IInteractable
{
    [HideInInspector]public NPC_StateMachine _NPC_StateMachine;
    [HideInInspector]public NPC_Animator NPCAnimator { get; set; }

    public NavMeshAgent Agent;
    //public Transform point;

    [Header("Все для преследования игрока")]
    public Transform player;
    [Tooltip("Радиус обнаружения игрока")]
    public float viewRadiusByEyes = 10f;
    public float viewRadiusAround = 3f;

    [Tooltip("Угол обзора")]
    public float viewAngle = 120f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        NPCAnimator = GetComponent<NPC_Animator>();
        
        
        _NPC_StateMachine = new NPC_StateMachine();
        _NPC_StateMachine.SetState(new PatrolState(this));
        _NPC_StateMachine.OnStateChanged += SwitchAnimations;

    }
    protected void Update()
    {
        _NPC_StateMachine.StateUpdate();
    }

    public virtual void Interact()
    {
        Debug.Log("Default Interaction");
    }

    protected void SwitchAnimations() 
    {
        NPCAnimator.SwitchAnimationState("IdleVariant", Random.Range(0, 3));
    }

    public bool CanSeePlayer()
    {
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        //Debug.Log(distanceToPlayer);
        if (viewRadiusAround > distanceToPlayer) return true;
        if (distanceToPlayer > viewRadiusByEyes) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2) return false;

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, viewRadiusByEyes, playerLayer | obstacleLayer))
        {
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
            {
                return true;
            }
        }
        return false;

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadiusByEyes);
        
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadiusAround);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadiusByEyes);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadiusByEyes);
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        return Quaternion.Euler(0, angleInDegrees + transform.eulerAngles.y, 0) * Vector3.forward;
    }

    public void SwitchSpeed(float AgentSpeed) 
    {
        Agent.speed = AgentSpeed;   
    }

}
