using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat
{
    public class ConeDeliveryHandler : DeliveryHandler
    {
        public DeliveryModuleData module;
        public PoolManager pool;
        public Transform muzzle;

        private void Awake()
        {
            if (muzzle == null)
            {
                muzzle = transform;
            }
        }

        public override void Fire(Vector3 direction, float damage)
        {
            if (module == null)
            {
                return;
            }

            Vector3 origin = muzzle.position;

            Vector3 flatDir = direction;
            flatDir.y = 0f;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowSector(origin, flatDir, module.angle, module.range, 0.25f, new Color(1f, 0.55f, 0.1f, 0.25f));
#endif

            float rangeSqr = module.range * module.range;
            float halfAngle = module.angle * 0.5f;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                Vector3 toEnemy = enemy.transform.position - origin;
                toEnemy.y = 0f;

                if (toEnemy.sqrMagnitude <= rangeSqr && Vector3.Angle(flatDir, toEnemy) <= halfAngle)
                {
                    enemy.TryApplyHit(damage, enemy.transform.position, onHit);
                }
            }

            if (module.coneVfx != null && pool != null)
            {
                GameObject vfx = pool.Get(module.coneVfx);
                vfx.transform.position = origin;
                float yaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;
                vfx.transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up);
                vfx.GetComponent<PooledVFX>().Play(pool);
            }
        }
    }
}
