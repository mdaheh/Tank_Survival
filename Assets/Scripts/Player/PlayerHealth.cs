using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private StatBlock statBlock;

    private float currentHealth;

    public float CurrentHealth => currentHealth;

    public event Action OnDeath;

    public void Init()
    {
        currentHealth = statBlock.maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }
}
