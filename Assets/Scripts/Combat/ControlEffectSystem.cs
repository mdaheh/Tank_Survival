using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class ControlEffectSystem : MonoBehaviour
    {
        private class ControlStatus
        {
            public EnemyController enemy;
            public float slowAmount;
            public int freezeStacks;
            public bool frozen;
            public float duration;
            public float elapsed;
        }

        private readonly List<ControlStatus> _statuses = new List<ControlStatus>();

        private void Awake()
        {
            GameServices.Register(this);
        }

        public void ApplySlow(EnemyController enemy, float slowAmount, float duration)
        {
            if (!IsValid(enemy))
            {
                return;
            }

            ControlStatus status = Find(enemy);
            if (status != null)
            {
                status.slowAmount = slowAmount;
                status.duration = duration;
                status.elapsed = 0f;
            }
            else
            {
                _statuses.Add(new ControlStatus
                {
                    enemy = enemy,
                    slowAmount = slowAmount,
                    duration = duration
                });
            }

            enemy.controlSpeedMultiplier = 1f - slowAmount;
        }

        public void ApplyFreeze(EnemyController enemy, float slowAmount, float duration, int freezeThreshold)
        {
            if (!IsValid(enemy))
            {
                return;
            }

            ControlStatus status = Find(enemy);
            if (status != null)
            {
                status.slowAmount = slowAmount;
                status.duration = duration;
                status.elapsed = 0f;
                status.freezeStacks++;
            }
            else
            {
                status = new ControlStatus
                {
                    enemy = enemy,
                    slowAmount = slowAmount,
                    duration = duration,
                    freezeStacks = 1
                };
                _statuses.Add(status);
            }

            if (status.freezeStacks >= freezeThreshold)
            {
                status.frozen = true;
            }

            enemy.controlSpeedMultiplier = status.frozen ? 0f : (1f - slowAmount);
        }

        public void RemoveFreeze(EnemyController enemy)
        {
            if (enemy == null)
            {
                return;
            }

            for (int i = _statuses.Count - 1; i >= 0; i--)
            {
                if (_statuses[i].enemy == enemy)
                {
                    _statuses.RemoveAt(i);
                    break;
                }
            }

            enemy.controlSpeedMultiplier = 1f;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public int GetFreezeStacks(EnemyController enemy)
        {
            ControlStatus status = enemy != null ? Find(enemy) : null;

            return status != null ? status.freezeStacks : 0;
        }

        public bool IsFrozen(EnemyController enemy)
        {
            ControlStatus status = enemy != null ? Find(enemy) : null;

            return status != null && status.frozen;
        }
#endif

        private void Update()
        {
            float dt = Time.deltaTime;

            for (int i = _statuses.Count - 1; i >= 0; i--)
            {
                ControlStatus status = _statuses[i];
                status.elapsed += dt;

                if (!IsValid(status.enemy) || status.elapsed >= status.duration)
                {
                    _statuses.RemoveAt(i);
                    if (status.enemy != null)
                    {
                        status.enemy.controlSpeedMultiplier = 1f;
                    }

                    continue;
                }

                status.enemy.controlSpeedMultiplier = status.frozen ? 0f : (1f - status.slowAmount);
            }
        }

        private ControlStatus Find(EnemyController enemy)
        {
            for (int i = 0; i < _statuses.Count; i++)
            {
                if (_statuses[i].enemy == enemy)
                {
                    return _statuses[i];
                }
            }

            return null;
        }

        private static bool IsValid(EnemyController enemy)
        {
            return enemy != null
                && enemy.health != null
                && enemy.health.currentHealth > 0f
                && enemy.gameObject.activeSelf;
        }
    }
}
