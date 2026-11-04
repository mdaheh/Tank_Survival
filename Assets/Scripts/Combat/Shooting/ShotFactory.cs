using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class ShotFactory
    {
        public SpawnRequest Build(DeliveryModuleData module, Vector3 position, Vector3 direction,
            float damage, int depth = 0)
        {
            BuildSnapshot buildSnapshot = new BuildSnapshot();
            buildSnapshot.damage = damage;
            buildSnapshot.speed = module.speed;
            buildSnapshot.hitModules = new IOnHit[] { new DamageModule() };

            DeliveryConfig config = new DeliveryConfig();
            config.range = module.range;
            config.angle = module.angle;
            config.maxTargets = module.maxTargets;
            config.continuous = module.duration == DeliveryModuleData.DeliveryDuration.Continuous;
            config.tickRate = module.tickRate;

            SpawnRequest request = new SpawnRequest();
            request.type = MapType(module.kind);
            request.position = position;
            request.direction = direction;
            request.buildSnapshot = buildSnapshot;
            request.config = config;
            request.depth = depth;

            return request;
        }

        private static DeliveryType MapType(DeliveryModuleData.DeliveryKind kind)
        {
            switch (kind)
            {
                case DeliveryModuleData.DeliveryKind.Hitscan:
                    return DeliveryType.Hitscan;
                case DeliveryModuleData.DeliveryKind.Cone:
                    return DeliveryType.Cone;
                default:
                    return DeliveryType.Projectile;
            }
        }
    }
}
