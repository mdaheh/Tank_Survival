using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        public float damage;
        public float speed;
        public float range;
        public Vector3 direction;
        public PoolManager pool;

        private const float HitRadiusSqr = 0.25f;

        private float _distanceTravelled;

        public void Init(float damage, float speed, Vector3 direction, PoolManager pool)
        {
            this.damage = damage;
            this.speed = speed;
            this.direction = direction;
            this.pool = pool;
        }

        private void Update()
        {
            if (pool == null)
            {
                return;
            }

            float step = speed * Time.deltaTime;
            transform.Translate(direction * step, Space.World);
            _distanceTravelled += step;

            if (_distanceTravelled >= range)
            {
                pool.Release(gameObject);
                return;
            }

            Vector3 position = transform.position;
            List<EnemyController> active = EnemyController.Active;
            for (int i = 0; i < active.Count; i++)
            {
                EnemyController enemy = active[i];
                if ((enemy.transform.position - position).sqrMagnitude <= HitRadiusSqr)
                {
                    enemy.health.TakeDamage(damage);
                    pool.Release(gameObject);
                    return;
                }
            }
        }

        public void OnGetFromPool()
        {
            _distanceTravelled = 0f;
            direction = Vector3.zero;
        }

        public void OnReleaseFromPool()
        {
            _distanceTravelled = 0f;
            direction = Vector3.zero;
        }
    }
}
