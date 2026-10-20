using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Enemy
{
    public class EnemyProjectile : MonoBehaviour, IPoolable
    {
        public float damage;
        public float speed = 10f;
        public float range;
        public Vector3 direction;
        public PoolManager pool;

        private IDamageable _targetDamageable;
        private Transform _target;
        private float _travelled;

        private const float HitRadiusSqr = 1f;

        public void Init(float damage, float range, Vector3 direction,
            PoolManager pool, Transform target)
        {
            this.damage = damage;
            this.range = range;
            this.direction = direction;
            this.pool = pool;
            _targetDamageable = target.GetComponent<IDamageable>();
            _target = target;
            _travelled = 0f;
        }

        private void Update()
        {
            if (pool == null)
                return;

            float step = speed * Time.deltaTime;
            transform.Translate(direction * step, Space.World);
            _travelled += step;

            if (_travelled >= range)
            {
                pool.Release(gameObject);
                return;
            }

            if (_target == null)
            {
                pool.Release(gameObject);
                return;
            }

            if ((transform.position - _target.position).sqrMagnitude <= HitRadiusSqr)
            {
                _targetDamageable.TakeDamage(damage);
                pool.Release(gameObject);
            }
        }

        public void OnGetFromPool()
        {
            _travelled = 0f;
            direction = Vector3.zero;
        }

        public void OnReleaseFromPool()
        {
            _travelled = 0f;
            direction = Vector3.zero;
        }
    }
}