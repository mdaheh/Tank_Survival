using System;
using System.Collections.Generic;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class ShotFactory
    {
        public SpawnRequest Build(DeliveryModuleData module, Vector3 position, Vector3 direction,
            float damage, int depth = 0)
        {
            return Build(module, Array.Empty<OnHitModuleData>(), position, direction, damage, depth);
        }

        public SpawnRequest Build(DeliveryModuleData module, OnHitModuleData[] onHitModules, Vector3 position,
            Vector3 direction, float damage, int depth = 0)
        {
            BuildSnapshot buildSnapshot = new BuildSnapshot();
            buildSnapshot.damage = damage;
            buildSnapshot.speed = module.speed;
            PopulateModules(buildSnapshot, onHitModules);

            DeliveryConfig config = new DeliveryConfig();
            config.range = module.range;
            config.angle = module.angle;
            config.maxTargets = module.maxTargets;
            config.continuous = module.duration == DeliveryModuleData.DeliveryDuration.Continuous;
            config.tickRate = module.tickRate;
            config.coneVfx = module.coneVfx;

            SpawnRequest request = new SpawnRequest();
            request.type = MapType(module.kind, module.duration);
            request.position = position;
            request.direction = direction;
            request.buildSnapshot = buildSnapshot;
            request.config = config;
            request.depth = depth;

            return request;
        }

        private static void PopulateModules(BuildSnapshot buildSnapshot, OnHitModuleData[] onHitModules)
        {
            List<OnHitModuleData> ordered = CollectModules(onHitModules);

            List<IOnSpawn> spawnModules = new List<IOnSpawn>();
            List<IOnFlight> flightModules = new List<IOnFlight>();
            List<IOnHit> hitModules = new List<IOnHit>();
            List<IOnPostHit> postHitModules = new List<IOnPostHit>();

            hitModules.Add(new DamageModule());

            for (int i = 0; i < ordered.Count; i++)
            {
                OnHitModuleData data = ordered[i];

                switch (data.kind)
                {
                    case OnHitModuleData.OnHitKind.Burn:
                        hitModules.Add(new BurnModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.Slow:
                        hitModules.Add(new SlowModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.Freeze:
                        hitModules.Add(new FreezeModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.Knockback:
                        hitModules.Add(new KnockbackModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.Explosion:
                        hitModules.Add(new ExplosionModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.Ground:
                        hitModules.Add(new GroundModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.Pierce:
                        {
                            PierceModule module = new PierceModule(data);
                            spawnModules.Add(module);
                            postHitModules.Add(module);
                        }
                        break;
                    case OnHitModuleData.OnHitKind.Homing:
                        flightModules.Add(new HomingModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.BeamSplit:
                        hitModules.Add(new BeamSplitModule(data));
                        break;
                    case OnHitModuleData.OnHitKind.LaserFocus:
                        hitModules.Add(new LaserFocusModule(data));
                        break;
                }
            }

            buildSnapshot.spawnModules = spawnModules.ToArray();
            buildSnapshot.flightModules = flightModules.ToArray();
            buildSnapshot.hitModules = hitModules.ToArray();
            buildSnapshot.postHitModules = postHitModules.ToArray();
        }

        private static List<OnHitModuleData> CollectModules(OnHitModuleData[] onHitModules)
        {
            List<OnHitModuleData> ordered = new List<OnHitModuleData>();

            if (onHitModules == null)
            {
                return ordered;
            }

            for (int i = 0; i < onHitModules.Length; i++)
            {
                if (onHitModules[i] != null)
                {
                    ordered.Add(onHitModules[i]);
                }
            }

            for (int i = 1; i < ordered.Count; i++)
            {
                OnHitModuleData key = ordered[i];
                int j = i - 1;
                while (j >= 0 && ordered[j].priority < key.priority)
                {
                    ordered[j + 1] = ordered[j];
                    j--;
                }

                ordered[j + 1] = key;
            }

            return ordered;
        }

        private static DeliveryType MapType(DeliveryModuleData.DeliveryKind kind,
            DeliveryModuleData.DeliveryDuration duration)
        {
            switch (kind)
            {
                case DeliveryModuleData.DeliveryKind.Hitscan:
                    return duration == DeliveryModuleData.DeliveryDuration.Continuous
                        ? DeliveryType.ContinuousHitscan
                        : DeliveryType.Hitscan;
                case DeliveryModuleData.DeliveryKind.Cone:
                    return DeliveryType.Cone;
                default:
                    return DeliveryType.Projectile;
            }
        }
    }
}
