using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour , IDamageable
{
    [SerializeField] private float MaxHealth;
    [SerializeField] private float CurrentHealth;


    public event Action OnDeath;
    public event Action<float> OnHealthChanged;
    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;
        
        CurrentHealth -= amount;
        OnHealthChanged?.Invoke(CurrentHealth);// можно передавать здоровье

        if (CurrentHealth <= 0) 
        {
            //Death
        }
        
    
    
    }

    private void Die() 
    {
        Debug.Log("умер");
        OnDeath?.Invoke();
    }

    public void Heal(float amount) 
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    
    
}
