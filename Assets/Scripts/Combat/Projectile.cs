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
        public OnHitDispatcher onHit;

        private const float HitRadiusSqr = 1.0f;

        private float _distanceTravelled;

        public void Init(float damage, float speed, float range, Vector3 direction, PoolManager pool, OnHitDispatcher onHit)
        {
            this.damage = damage;
            this.speed = speed;
            this.range = range;
            this.direction = direction;
            this.pool = pool;
            this.onHit = onHit;
        }

        private void Update()
        {
            if (pool == null)
            {
                return;
            }

            float step = speed * Time.deltaTime;
            Vector3 previous = transform.position;
            transform.Translate(direction * step, Space.World);
            _distanceTravelled += step;

            if (_distanceTravelled >= range)
            {
                pool.Release(gameObject);
                return;
            }

            Vector3 current = transform.position;
            List<EnemyController> active = EnemyController.Active;
            for (int i = 0; i < active.Count; i++)
            {
                EnemyController enemy = active[i];
                float distanceSqr = DistanceToSegmentXZSqr(enemy.transform.position, previous, current);
                if (distanceSqr <= HitRadiusSqr)
                {
                    enemy.TryApplyHit(damage, enemy.transform.position, onHit);
                    pool.Release(gameObject);
                    return;
                }
            }
        }

        private static float DistanceToSegmentXZSqr(Vector3 point, Vector3 a, Vector3 b)
        {
            float abX = b.x - a.x;
            float abZ = b.z - a.z;
            float lengthSqr = abX * abX + abZ * abZ;
            if (lengthSqr <= 0f)
            {
                float pointX = point.x - a.x;
                float pointZ = point.z - a.z;
                return pointX * pointX + pointZ * pointZ;
            }

            float t = ((point.x - a.x) * abX + (point.z - a.z) * abZ) / lengthSqr;
            t = Mathf.Clamp01(t);

            float qX = a.x + t * abX;
            float qZ = a.z + t * abZ;

            float dx = point.x - qX;
            float dz = point.z - qZ;
            return dx * dx + dz * dz;
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
