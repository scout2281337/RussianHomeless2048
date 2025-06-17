using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private readonly NPC_Brain npc;

    public ChaseState(NPC_Brain npc) 
    {
        this.npc = npc;
    }
    public void Enter()
    {
        Debug.Log("current state : Chase");
        npc.SwitchSpeed(5f);
    }

    public void Exit()
    {
        Debug.Log("leaving current state");
    }
    public void Update()
    {
        npc.Agent.SetDestination(npc.player.position);
        if (Vector3.Distance(npc.transform.position, npc.player.position) > 15) 
        {
            npc._NPC_StateMachine.SetState(new PatrolState(npc));    
        }
        if (Vector3.Distance(npc.transform.position, npc.player.position) < 1)
        {
            //npc._NPC_StateMachine.SetState(new PatrolState(npc)); Состояние боя
        }
    }


}
