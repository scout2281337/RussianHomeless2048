using UnityEngine;
using UnityEngine.AI;


public class PatrolState : IState
{
    private readonly NPC_Brain npc;
    private float PatrolRadius = 30f;
    private float waitTime = 5f;
    private float waitTimer = 0f;
    private bool destinationSet = false;

    public PatrolState(NPC_Brain npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        Debug.Log("current state : Patrol");
        PickNewDestination();
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            npc._NPC_StateMachine.SetState(new IdleState(npc));
        }
        if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                PickNewDestination();
            }
        }
    }
    public void Exit()
    {
        Debug.Log("leaving current state");
        npc.Agent.ResetPath();
    }

    private void PickNewDestination() 
    {
        
        waitTime = Random.Range(0.1f, 10f);
        
        Vector3 randomDirection = Random.insideUnitSphere * PatrolRadius;
        randomDirection += npc.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, PatrolRadius, NavMesh.AllAreas))
        {
            npc.Agent.SetDestination(hit.position);
            destinationSet = true;
        }
    }
}
