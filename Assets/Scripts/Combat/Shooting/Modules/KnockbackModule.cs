using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class KnockbackModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        public KnockbackModule(OnHitModuleData data)
        {
            _data = data;
        }

        public void OnHit(ShotContext ctx, Target target)
        {
            EnemyController enemy = target?.enemy;
            if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
            {
                return;
            }

            Vector3 dir = enemy.transform.position - ctx.hitPoint;
            dir.y = 0f;
            dir.Normalize();

            enemy.ApplyKnockback(dir * (_data.force * _data.damageScaling), _data.duration);
        }
    }
}
