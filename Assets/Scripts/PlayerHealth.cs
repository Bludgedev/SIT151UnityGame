using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    public float HealthPercent => CurrentHealth / maxHealth;

    

    public event Action OnDeath;
    public event Action OnHealed;
    public event Action<float> OnDamageTaken;
    public event Action<float> OnHealthPercentChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        
    }


    //====================================================
    // DAMAGE
    //====================================================

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0f)
            return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

        OnDamageTaken?.Invoke(damage);
        OnHealthPercentChanged?.Invoke(HealthPercent);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    //====================================================
    // HEALING (for pickups)
    //====================================================

    public void Heal(float amount)
    {
        if (CurrentHealth <= 0f)
            return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

        OnHealed?.Invoke();
        OnHealthPercentChanged?.Invoke(HealthPercent);
    }

    //====================================================
    // DEATH
    //====================================================

    private void Die()
    {
        OnDeath?.Invoke();
    }

    //=============================
    //   collision detection effects
    //=============================
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageDealer>(out var damageDealer))
        {
            TakeDamage(damageDealer.Damage);
        }
    }

}