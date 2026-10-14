using TankSurvival.Core;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class ProjectileDeliveryHandler : DeliveryHandler
    {
        public DeliveryModuleData module;
        public GameObject projectilePrefab;
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
            if (module == null || projectilePrefab == null || pool == null)
            {
                return;
            }

            float halfSpread = module.spreadAngle * 0.5f;

            for (int i = 0; i < module.pelletCount; i++)
            {
                GameObject projectile = pool.Get(projectilePrefab);
                projectile.transform.position = muzzle.position;

                float spread = Random.Range(-halfSpread, halfSpread);
                Vector3 spreadDirection = Quaternion.AngleAxis(spread, Vector3.up) * direction;

                projectile.GetComponent<Projectile>().Init(damage, module.speed, module.range, spreadDirection, pool, onHit);
            }
        }
    }
}
