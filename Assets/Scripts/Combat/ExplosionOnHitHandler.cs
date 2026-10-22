using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat
{
    public class ExplosionOnHitHandler : MonoBehaviour, IHitHandler
    {
        private PoolManager _pool;

        private PoolManager Pool =>
            _pool != null ? _pool : (_pool = GameServices.Get<PoolManager>());

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null)
                return;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowCircle(position, module.radius, 0.3f, new Color(1f, 0.25f, 0.2f, 0.5f));
#endif

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

            if (module.explosionVfx != null && Pool != null)
            {
                GameObject vfx = Pool.Get(module.explosionVfx);
                vfx.transform.position = position;
                vfx.GetComponent<PooledVFX>().Play(Pool);
            }
        }
    }
}