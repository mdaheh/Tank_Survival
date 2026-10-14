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
        public DeliveryHandler hitscanDelivery;

        private float _cooldown;

        private void Update()
        {
            _cooldown -= Time.deltaTime;

            if (turret == null || turret.delivery == null)
            {
                return;
            }

            DeliveryHandler active = turret.delivery.kind == DeliveryModuleData.DeliveryKind.Hitscan
                ? hitscanDelivery
                : delivery;
            if (active == null)
            {
                return;
            }

            Vector3 position = transform.position;
            float closestDistanceSqr = turret.delivery.range * turret.delivery.range;
            EnemyController closest = null;

            List<EnemyController> enemies = EnemyController.Active;
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyController enemy = enemies[i];
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
                active.Fire(direction);
                _cooldown = 1f / turret.delivery.aps;
            }
        }
    }
}
