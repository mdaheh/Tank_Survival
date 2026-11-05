using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class InstantHitscanController
    {
        private const float HitRadiusSqr = 1f;

        public void Tick(ShotContext ctx, float dt)
        {
            ctx.age += dt;

            Vector3 origin = ctx.position;
            Vector3 end = origin + ctx.direction * ctx.config.range;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowBeam(origin, end, 0.15f, new Color(1f, 0.9f, 0.2f, 0.6f));
#endif

            float minAlong = float.NegativeInfinity;

            for (int i = 0; i < ctx.config.maxTargets; i++)
            {
                float along;
                EnemyController enemy = FindNext(ctx, origin, end, minAlong, true, out along);
                if (enemy == null)
                {
                    break;
                }

                minAlong = along;

                Target target = BuildTarget(enemy);
                ctx.hitTargets.Add(target);
                ctx.hitPoint = target.position;
                RunHitModules(ctx, target);
            }

            ctx.keepAlive = false;

            IOnPostHit[] postHitModules = ctx.buildSnapshot.postHitModules;
            for (int i = 0; i < postHitModules.Length; i++)
            {
                postHitModules[i].OnPostHit(ctx);
            }

            if (!ctx.keepAlive)
            {
                ctx.destroyReason = DestroyReason.Hit;
                DestroyShot(ctx);
            }
        }

        private static EnemyController FindNext(ShotContext ctx, Vector3 origin, Vector3 end, float minAlong,
            bool skipHitTargets, out float along)
        {
            EnemyController best = null;
            float bestAlong = float.PositiveInfinity;
            along = 0f;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

                if (skipHitTargets && IsAlreadyHit(ctx, enemy))
                {
                    continue;
                }

                Vector3 point = enemy.transform.position;
                if (!IsOnBeam(origin, end, point))
                {
                    continue;
                }

                float candidateAlong = Vector3.Dot(point - origin, ctx.direction);
                if (candidateAlong <= minAlong || candidateAlong >= bestAlong)
                {
                    continue;
                }

                bestAlong = candidateAlong;
                best = enemy;
            }

            if (best != null)
            {
                along = bestAlong;
            }

            return best;
        }

        private static bool IsOnBeam(Vector3 origin, Vector3 end, Vector3 point)
        {
            Vector3 segment = end - origin;
            float lengthSqr = segment.sqrMagnitude;
            float t = lengthSqr > 0f ? Mathf.Clamp01(Vector3.Dot(point - origin, segment) / lengthSqr) : 0f;
            Vector3 closest = origin + segment * t;

            return (point - closest).sqrMagnitude <= HitRadiusSqr;
        }

        private static bool IsAlreadyHit(ShotContext ctx, EnemyController enemy)
        {
            for (int i = 0; i < ctx.hitTargets.Count; i++)
            {
                if (ctx.hitTargets[i].damageable == enemy.Damageable)
                {
                    return true;
                }
            }

            return false;
        }

        private static Target BuildTarget(EnemyController enemy)
        {
            Target target = new Target();
            target.position = enemy.transform.position;
            target.damageable = enemy.Damageable;
            target.faction = Faction.Enemy;

            return target;
        }

        private static void RunHitModules(ShotContext ctx, Target target)
        {
            IOnHit[] hitModules = ctx.buildSnapshot.hitModules;
            for (int i = 0; i < hitModules.Length; i++)
            {
                hitModules[i].OnHit(ctx, target);
            }
        }

        private static void DestroyShot(ShotContext ctx)
        {
            IOnDestroy[] destroyModules = ctx.buildSnapshot.destroyModules;
            for (int i = 0; i < destroyModules.Length; i++)
            {
                destroyModules[i].OnDestroy(ctx);
            }

            ctx.state = ShotState.Dead;
        }
    }
}
