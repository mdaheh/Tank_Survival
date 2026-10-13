using System.Collections.Generic;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class TurretController : MonoBehaviour
    {
        public TurretData turret;
        public DeliveryHandler delivery;

        private float _cooldown;

        private void Update()
        {
            _cooldown -= Time.deltaTime;

            if (turret == null || turret.delivery == null || delivery == null)
            {
                return;
            }

            Vector3 position = transform.position;
            float closestDistanceSqr = turret.delivery.range * turret.delivery.range;
            EnemyController closest = null;

            List<EnemyController> active = EnemyController.Active;
            for (int i = 0; i < active.Count; i++)
            {
                EnemyController enemy = active[i];
                float distanceSqr = (enemy.transform.position - position).sqrMagnitude;
                if (distanceSqr <= closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closest = enemy;
                }
            }

            if (closest != null && _cooldown <= 0f)
            {
                Vector3 direction = (closest.transform.position - position).normalized;
                delivery.Fire(direction);
                _cooldown = 1f / turret.delivery.aps;
            }
        }
    }
}
