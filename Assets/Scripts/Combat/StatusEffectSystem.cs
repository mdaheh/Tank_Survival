using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class StatusEffectSystem : MonoBehaviour
    {
        private sealed class BurnStatus
        {
            public EnemyController enemy;
            public EnemyHealth health;
            public float tickDamage;
            public float tickRate;
            public float duration;
            public float tickTimer;
            public float elapsed;
        }

        private readonly List<BurnStatus> burns = new List<BurnStatus>();

        private void Awake()
        {
            GameServices.Register(this);
        }

        public void ApplyBurn(EnemyController enemy, float tickDamage, float tickRate, float duration)
        {
            if (enemy == null)
                return;

            EnemyHealth health = enemy.health;
            if (health == null || health.currentHealth <= 0 || !enemy.gameObject.activeSelf)
                return;

            for (int i = 0; i < burns.Count; i++)
            {
                BurnStatus existing = burns[i];
                if (existing.enemy == enemy)
                {
                    existing.tickDamage = tickDamage;
                    existing.tickRate = tickRate;
                    existing.duration = duration;
                    existing.elapsed = 0f;
                    existing.tickTimer = 0f;
                    return;
                }
            }

            BurnStatus status = new BurnStatus
            {
                enemy = enemy,
                health = health,
                tickDamage = tickDamage,
                tickRate = tickRate,
                duration = duration,
                tickTimer = 0f,
                elapsed = 0f
            };
            burns.Add(status);
        }

        public void RemoveBurn(EnemyController enemy)
        {
            if (enemy == null)
                return;

            for (int i = burns.Count - 1; i >= 0; i--)
            {
                if (burns[i].enemy == enemy)
                {
                    burns.RemoveAt(i);
                    return;
                }
            }
        }

#if UNITY_EDITOR || DEBUG
        public int ActiveBurnCount => burns.Count;

        public float GetBurnRemaining(EnemyController enemy)
        {
            if (enemy == null)
                return 0f;

            for (int i = 0; i < burns.Count; i++)
            {
                BurnStatus status = burns[i];
                if (status.enemy == enemy)
                {
                    return Mathf.Max(0f, status.duration - status.elapsed);
                }
            }

            return 0f;
        }
#endif

        private void Update()
        {
            float dt = Time.deltaTime;

            for (int i = burns.Count - 1; i >= 0; i--)
            {
                BurnStatus status = burns[i];

                status.elapsed += dt;
                status.tickTimer += dt;

                if (status.health.currentHealth <= 0 ||
                    !status.enemy.gameObject.activeSelf ||
                    status.elapsed >= status.duration)
                {
                    burns.RemoveAt(i);
                    continue;
                }

                if (status.tickTimer >= status.tickRate)
                {
                    status.tickTimer -= status.tickRate;
                    status.health.TakeDamage(status.tickDamage);
                }
            }
        }
    }
}