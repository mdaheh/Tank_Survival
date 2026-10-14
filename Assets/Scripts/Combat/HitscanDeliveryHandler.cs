using System.Collections.Generic;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class HitscanDeliveryHandler : DeliveryHandler
    {
        public DeliveryModuleData module;
        public Transform muzzle;

        private const float HitRadiusSqr = 1.0f;

        private readonly List<EnemyController> _candidates = new List<EnemyController>();

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
            Vector3 end = origin + direction * module.range;

            _candidates.Clear();
            List<EnemyController> active = EnemyController.Active;
            for (int i = 0; i < active.Count; i++)
            {
                EnemyController enemy = active[i];
                float distanceSqr = DistanceToSegmentXZSqr(enemy.transform.position, origin, end);
                if (distanceSqr <= HitRadiusSqr)
                {
                    _candidates.Add(enemy);
                }
            }

            int toHit = module.maxTargets;
            if (toHit > _candidates.Count)
            {
                toHit = _candidates.Count;
            }

            for (int i = 0; i < toHit; i++)
            {
                int nearest = i;
                float nearestAlong = AlongBeamXZ(_candidates[i].transform.position, origin, end);
                for (int j = i + 1; j < _candidates.Count; j++)
                {
                    float along = AlongBeamXZ(_candidates[j].transform.position, origin, end);
                    if (along < nearestAlong)
                    {
                        nearestAlong = along;
                        nearest = j;
                    }
                }

                if (nearest != i)
                {
                    EnemyController swap = _candidates[i];
                    _candidates[i] = _candidates[nearest];
                    _candidates[nearest] = swap;
                }

                _candidates[i].Damageable.TakeDamage(damage);
                if (onHit != null)
                {
                    onHit.ProcessHit(_candidates[i].transform.position, _candidates[i]);
                }
            }
        }

        private static float AlongBeamXZ(Vector3 point, Vector3 a, Vector3 b)
        {
            float abX = b.x - a.x;
            float abZ = b.z - a.z;
            float lengthSqr = abX * abX + abZ * abZ;
            if (lengthSqr <= 0f)
            {
                return 0f;
            }

            return (point.x - a.x) * abX + (point.z - a.z) * abZ;
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
    }
}
