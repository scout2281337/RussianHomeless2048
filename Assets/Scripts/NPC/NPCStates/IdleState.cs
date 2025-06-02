using UnityEngine;


public class IdleState : IState
{
    private readonly NPC_Brain npc;

    public IdleState(NPC_Brain npc)
    {
        this.npc = npc;
    }


    public void Enter()
    {
        Debug.Log("current state : Idle");    
    }
    public void Update()
    {
        //логика выхода или проверки состояния 
        if (Input.GetKey(KeyCode.E)) 
        {
            npc._NPC_StateMachine.SetState(new PatrolState(npc));
        }
    }
    public void Exit()
    {
        Debug.Log("leaving current state");
    }
}
