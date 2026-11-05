using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class HomingModule : IOnFlight
    {
        private const float MinDirectionSqr = 1e-6f;
        private const float TurnSpeedDegPerStrength = 60f;

        private readonly OnHitModuleData _data;

        public HomingModule(OnHitModuleData data)
        {
            _data = data;
        }

        public void OnFlight(ShotContext ctx, float dt)
        {
            if (_data == null)
            {
                return;
            }

            EnemyController nearest = FindNearest(ctx);
            if (nearest == null)
            {
                return;
            }

            Vector3 desired = nearest.transform.position - ctx.position;
            desired.y = 0f;
            if (desired.sqrMagnitude < MinDirectionSqr)
            {
                return;
            }

            desired.Normalize();

            float turnDegrees = _data.homingStrength * TurnSpeedDegPerStrength * dt;
            Vector3 current = ctx.direction;
            if (current.sqrMagnitude < MinDirectionSqr)
            {
                ctx.direction = desired;
                return;
            }

            ctx.direction = Vector3.RotateTowards(current, desired, turnDegrees * Mathf.Deg2Rad, 0f).normalized;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowCircle(ctx.position, _data.homingRadius, dt, new Color(0.2f, 0.8f, 1f, 0.35f));
#endif
        }

        private EnemyController FindNearest(ShotContext ctx)
        {
            float radiusSqr = _data.homingRadius * _data.homingRadius;
            EnemyController best = null;
            float bestSqr = radiusSqr;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

                float sqr = (enemy.transform.position - ctx.position).sqrMagnitude;
                if (sqr <= bestSqr)
                {
                    bestSqr = sqr;
                    best = enemy;
                }
            }

            return best;
        }
    }
}
