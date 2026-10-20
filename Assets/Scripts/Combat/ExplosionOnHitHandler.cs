using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class ExplosionOnHitHandler : MonoBehaviour, IHitHandler
    {
        public PoolManager pool;

        private readonly List<EnemyController> _buffer = new List<EnemyController>(32);

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null)
                return;

            _buffer.Clear();
            _buffer.AddRange(EnemyController.Active);

            float radiusSq = module.radius * module.radius;
            for (int i = 0; i < _buffer.Count; i++)
            {
                EnemyController enemy = _buffer[i];
                if (enemy == null)
                    continue;

                Vector3 enemyPosition = enemy.transform.position;
                float dx = enemyPosition.x - position.x;
                float dz = enemyPosition.z - position.z;
                if (dx * dx + dz * dz <= radiusSq)
                    enemy.Damageable.TakeDamage(module.damage);
            }

            if (module.explosionVfx != null && pool != null)
            {
                GameObject vfx = pool.Get(module.explosionVfx);
                vfx.transform.position = position;
                vfx.GetComponent<PooledVFX>().Play(pool);
            }
        }
    }
}