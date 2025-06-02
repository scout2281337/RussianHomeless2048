using UnityEngine;


public class MoveToTargetState : IState
{
    private readonly NPC_Brain npc;
    private readonly Transform target;
    public MoveToTargetState(NPC_Brain npc, Transform target)
    {
        this.npc = npc;
        this.target = target;
    }


    public void Enter()
    {
        Debug.Log("current state : MoveToTarget");
        npc.Agent.SetDestination(target.position);
    }
    public void Update()
    {
        float distance = Vector3.Distance(npc.transform.position, target.position);
        if (distance < 0.5) 
        {
            npc._NPC_StateMachine.SetState(new IdleState(npc));
        }
    }
    public void Exit()
    {
        Debug.Log("leaving current state");
    }
}
