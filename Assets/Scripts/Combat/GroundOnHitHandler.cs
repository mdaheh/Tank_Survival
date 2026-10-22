using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class GroundOnHitHandler : MonoBehaviour, IHitHandler
    {
        private PoolManager _pool;
        private OnHitDispatcher _onHit;

        private PoolManager Pool =>
            _pool != null ? _pool : (_pool = GameServices.Get<PoolManager>());

        private OnHitDispatcher OnHit =>
            _onHit != null ? _onHit : (_onHit = GameServices.Get<OnHitDispatcher>());

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            HandleHit(module, position, target, 0);
        }

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target, int depth)
        {
            if (module == null || module.groundVfx == null || Pool == null || OnHit == null)
            {
                return;
            }

            GameObject zone = Pool.Get(module.groundVfx);
            zone.transform.position = position;
            zone.GetComponent<GroundZone>().Init(module, position, Pool, OnHit, depth);
        }
    }
}
