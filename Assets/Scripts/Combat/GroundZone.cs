using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat
{
    public class GroundZone : MonoBehaviour, IPoolable
    {
        private PoolManager _pool;
        private OnHitDispatcher _onHit;
        private float _radius;
        private float _tickDamage;
        private float _tickRate;
        private float _duration;
        private int _depth;
        private float _elapsed;
        private float _tickTimer;

        public void Init(OnHitModuleData module, Vector3 position, PoolManager pool, OnHitDispatcher onHit, int depth)
        {
            _pool = pool;
            _onHit = onHit;
            _radius = module.radius;
            _tickDamage = module.tickDamage;
            _tickRate = module.tickRate;
            _duration = module.duration;
            _depth = depth;
            _elapsed = 0f;
            _tickTimer = 0f;
            transform.position = position;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DevVisualizer.ShowCircle(position, module.radius, module.duration, new Color(0.4f, 0.9f, 0.4f, 0.35f));
#endif
        }

        public void OnGetFromPool()
        {
            _pool = null;
            _elapsed = 0f;
            _tickTimer = 0f;
        }

        public void OnReleaseFromPool()
        {
            _pool = null;
            _elapsed = 0f;
            _tickTimer = 0f;
        }

        private void Update()
        {
            if (_pool == null)
            {
                return;
            }

            float dt = Time.deltaTime;
            _elapsed += dt;
            _tickTimer += dt;

            if (_elapsed >= _duration)
            {
                _pool.Release(gameObject);
                _pool = null;
                return;
            }

            if (_tickRate > 0f && _tickTimer >= _tickRate)
            {
                _tickTimer -= _tickRate;

                for (int i = 0; i < EnemyController.Active.Count; i++)
                {
                    EnemyController enemy = EnemyController.Active[i];
                    if (enemy == null)
                    {
                        continue;
                    }

                    float dx = enemy.transform.position.x - transform.position.x;
                    float dz = enemy.transform.position.z - transform.position.z;
                    if (dx * dx + dz * dz > _radius * _radius)
                    {
                        continue;
                    }

                    enemy.TryApplyHit(_tickDamage, enemy.transform.position, _onHit, _depth + 1);
                }
            }
        }
    }
}
