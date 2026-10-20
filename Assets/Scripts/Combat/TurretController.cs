using System.Collections.Generic;
using TankSurvival.Core;
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
        public DeliveryHandler coneDelivery;
        public OnHitDispatcher onHit;
        public StatBlock statBlock;

        private float _cooldown;

        private void Start()
        {
            if (onHit == null)
            {
                return;
            }

            if (turret != null)
            {
                onHit.SetModules(turret.onHitModules);
            }

            if (delivery != null)
            {
                delivery.onHit = onHit;
            }

            if (hitscanDelivery != null)
            {
                hitscanDelivery.onHit = onHit;
            }

            if (coneDelivery != null)
            {
                coneDelivery.onHit = onHit;
            }
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;

            if (turret == null || turret.delivery == null)
            {
                return;
            }

            DeliveryHandler active;
            if (turret.delivery.kind == DeliveryModuleData.DeliveryKind.Hitscan)
            {
                active = hitscanDelivery;
            }
            else if (turret.delivery.kind == DeliveryModuleData.DeliveryKind.Cone)
            {
                active = coneDelivery;
            }
            else
            {
                active = delivery;
            }

            if (active == null)
            {
                return;
            }

            bool continuous = turret.delivery.duration == DeliveryModuleData.DeliveryDuration.Continuous;
            if (continuous && turret.delivery.tickRate <= 0f)
            {
                return;
            }

            float interval = continuous
                ? 1f / turret.delivery.tickRate
                : 1f / statBlock.fireRate;

            Vector3 position = transform.position;
            float closestDistanceSqr = turret.delivery.range * turret.delivery.range;
            EnemyController closest = null;

            List<EnemyController> enemies = EnemyController.Active;
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyController enemy = enemies[i];
                if (enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

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
                active.Fire(direction, statBlock.damage);
                _cooldown = interval;
            }
        }
    }
}
