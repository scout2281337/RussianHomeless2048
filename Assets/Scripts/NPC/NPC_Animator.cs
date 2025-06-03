using UnityEngine;

public class NPC_Animator : MonoBehaviour
{
    private Animator animator;
    private NPC_Brain NPC_Brain;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        NPC_Brain = GetComponent<NPC_Brain>();
        if (animator == null ) 
        {
            Debug.Log("Аниматор не найден");
        }
    }

    public virtual void SetSpeed()
    { 
        if (animator != null)
        {
            float speed = NPC_Brain.Agent.velocity.magnitude; 
            animator.SetFloat("Speed", speed);
        }
    }

    private void Update()
    {
        SetSpeed();
    }

    public void SwitchAnimationState(string nameOfVariable, int Value) 
    {
        animator.SetInteger(nameOfVariable, Value);
    }
}
