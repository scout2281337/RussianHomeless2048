using UnityEngine;


public class PatrolState : IState
{
    private readonly NPC_Brain npc;

    public PatrolState(NPC_Brain npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        Debug.Log("current state : Patrol");    
    }
    public void Update()
    {
        //логика выхода или проверки состояния 
    }
    public void Exit()
    {
        Debug.Log("leaving current state");
    }
}
