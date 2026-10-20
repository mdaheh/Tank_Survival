using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class ExplosionOnHitHandler : MonoBehaviour, IHitHandler
    {
        public PoolManager pool;

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null)
                return;

            float radiusSq = module.radius * module.radius;
            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
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