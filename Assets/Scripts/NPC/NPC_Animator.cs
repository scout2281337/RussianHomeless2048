using UnityEngine;

public class NPC_Animator : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null ) 
        {
            Debug.Log("Аниматор не найден");
        }
    }

    public virtual void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public virtual void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public virtual void TriggerDie()
    {
        animator.SetTrigger("Die");
    }

    public virtual void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
}
