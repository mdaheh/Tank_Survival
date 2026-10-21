using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class GroundOnHitHandler : MonoBehaviour, IHitHandler
    {
        public PoolManager pool;
        public OnHitDispatcher onHit;

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            HandleHit(module, position, target, 0);
        }

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target, int depth)
        {
            if (module == null || module.groundVfx == null || pool == null || onHit == null)
            {
                return;
            }

            GameObject zone = pool.Get(module.groundVfx);
            zone.transform.position = position;
            zone.GetComponent<GroundZone>().Init(module, position, pool, onHit, depth);
        }
    }
}
