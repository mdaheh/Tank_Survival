using System.Collections.Generic;
using TankSurvival.Combat.Shooting;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class TurretController : MonoBehaviour
    {
        public TurretData turret;
        public ShotSystem shotSystem;
        public OnHitDispatcher onHit;
        public StatBlock statBlock;

        private readonly ShotFactory _factory = new ShotFactory();
        private int _continuousShotId = -1;

        private float _cooldown;

        private void Start()
        {
            onHit.SetModules(turret.onHitModules);
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;

            if (turret == null || turret.delivery == null)
            {
                return;
            }

            bool continuous = turret.delivery.duration == DeliveryModuleData.DeliveryDuration.Continuous;
            if (continuous && turret.delivery.tickRate <= 0f)
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

            if (continuous)
            {
                if ((_continuousShotId == -1 || !shotSystem.IsAlive(_continuousShotId)) && closest != null)
                {
                    Vector3 dir = (closest.transform.position - position).normalized;
                    _continuousShotId = shotSystem.Fire(_factory.Build(turret.delivery, position, dir, statBlock.damage));
                }

                return;
            }

            if (closest == null || _cooldown > 0f)
            {
                return;
            }

            Vector3 shotDirection = (closest.transform.position - position).normalized;
            shotSystem.Fire(_factory.Build(turret.delivery, position, shotDirection, statBlock.damage));
            _cooldown = 1f / statBlock.fireRate;
        }
    }
}
