using System;
using TankSurvival.Core;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Enemy
{
    public class EnemyHealth : MonoBehaviour, IDamageable, IPoolable
    {
        public EnemyData data;
        public float healthMultiplier = 1f;
        public float currentHealth;

        public event Action OnDeath;

        public void Init(EnemyData data)
        {
            this.data = data;
            currentHealth = data.health * healthMultiplier;
        }

        public void TakeDamage(float amount)
        {
            if (currentHealth <= 0f)
            {
                return;
            }

            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                OnDeath?.Invoke();
            }
        }

        public void OnGetFromPool()
        {
            if (data == null)
            {
                return;
            }

            currentHealth = data.health * healthMultiplier;
        }

        public void OnReleaseFromPool()
        {
            OnDeath = null;
        }
    }
}
