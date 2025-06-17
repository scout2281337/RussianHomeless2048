using UnityEngine;

public class Test_MoveEnemy : MonoBehaviour
{
    private Ray ray;
    public NPC_Brain brain;
    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 500)) 
        {
            brain.Agent.SetDestination(hitInfo.point);
        }
        

    }

}
