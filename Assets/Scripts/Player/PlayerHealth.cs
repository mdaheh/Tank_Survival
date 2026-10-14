using System;
using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private StatBlock statBlock;

        private float currentHealth;

        public float CurrentHealth => currentHealth;

        public event Action OnDeath;

        private void Awake()
        {
            statBlock.MaxHealthIncreased += delta => currentHealth += delta;
        }

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
}
