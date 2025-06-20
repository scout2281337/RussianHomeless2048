using UnityEngine;
using UnityEngine.AI;

public class NPC_AgroBrain : NPC_Brain
{
    //public Transform player ;
    //public float viewRadius = 10f;
    //public float viewAngle = 120f;
    //public LayerMask playerLayer;
    //public LayerMask obstacleLayer;
    
    
    
    protected override void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        _NPC_StateMachine = new NPC_StateMachine();
        _NPC_StateMachine.SetState(new PatrolState(this));
        _NPC_StateMachine.OnStateChanged += SwitchAnimations;
        NPCAnimator = GetComponent<NPC_Animator>();

    }

    //public bool CanSeePlayer() 
    //{
    //    Vector3 directionToPlayer = (player.position - transform.position).normalized;
    //    float distanceToPlayer = Vector3.Distance(directionToPlayer, transform.position);

    //    if (distanceToPlayer > viewRadius) return false;

    //    float angle = Vector3.Angle(transform.forward, directionToPlayer);
    //    if (angle > viewAngle / 2) return false;

    //    if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, viewRadius, playerLayer | obstacleLayer))
    //    {
    //        if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;

    //}
    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, viewRadius);

    //    Vector3 viewAngleA = DirFromAngle(-viewAngle / 2);
    //    Vector3 viewAngleB = DirFromAngle(viewAngle / 2);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
    //    Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    //}

    Vector3 DirFromAngle(float angleInDegrees)
    {
        return Quaternion.Euler(0, angleInDegrees + transform.eulerAngles.y, 0) * Vector3.forward;
    }



}
