using UnityEngine;
using UnityEngine.AI;

public class PanicState : IState
{
    private NPC_Brain npc;
    private float panicDuration = 25f;
    private float timer;

    public PanicState(NPC_Brain npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        Debug.Log("PANIC!!!");
        timer = 0f;
        RunAwayFromPlayer();
        npc.SwitchSpeed(5f);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        // если дошёл до точки — найти новую точку убегания
        if (!npc.Agent.pathPending && npc.Agent.remainingDistance < 0.5f)
        {
            RunAwayFromPlayer();
        }

        // выйти из паники через N секунд
        if (timer > panicDuration)
        {
            npc._NPC_StateMachine.SetState(new PatrolState(npc));
        }
    }

    public void Exit()
    {
        npc.Agent.ResetPath();
    }

    private void RunAwayFromPlayer()
    {
        Vector3 directionAway = (npc.transform.position - npc.player.transform.position).normalized;
        Vector3 runPoint = npc.transform.position + directionAway * 10f;

        if (NavMesh.SamplePosition(runPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            npc.Agent.SetDestination(hit.position);
        }
    }
}
