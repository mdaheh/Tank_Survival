using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class ContinuousHitscanController
    {
        private const float HitRadiusSqr = 1f;
        private const float MinDirectionSqr = 1e-6f;

        public void Tick(ShotContext ctx, float dt)
        {
            ctx.age += dt;

            if (ctx.state == ShotState.Spawn)
            {
                ctx.state = ShotState.Flight;
            }

            if (ctx.state != ShotState.Flight)
            {
                return;
            }

            EnemyController enemy = FindNearest(ctx);
            if (enemy == null)
            {
                ctx.destroyReason = DestroyReason.External;
                DestroyShot(ctx);
                return;
            }

            if (ctx.currentTarget == null)
            {
                ctx.currentTarget = new Target();
            }

            ctx.currentTarget.position = enemy.transform.position;
            ctx.currentTarget.damageable = enemy.Damageable;
            ctx.currentTarget.faction = 0;
            ctx.currentTarget.enemy = enemy;

            Vector3 toTarget = ctx.currentTarget.position - ctx.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > MinDirectionSqr)
            {
                ctx.direction = toTarget.normalized;
            }

            Vector3 origin = ctx.position;
            Vector3 end = origin + ctx.direction * ctx.config.range;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowBeam(origin, end, 0.15f, new Color(1f, 0.9f, 0.2f, 0.6f));
#endif

            if (ctx.age >= ctx.nextTickAt)
            {
                ctx.nextTickAt += ctx.config.tickRate;
                RunTick(ctx, origin, end);
            }

            if (ctx.lifetime > 0f && ctx.age >= ctx.lifetime)
            {
                ctx.destroyReason = DestroyReason.Expired;
                DestroyShot(ctx);
            }
        }

        private static void RunTick(ShotContext ctx, Vector3 origin, Vector3 end)
        {
            float minAlong = float.NegativeInfinity;

            for (int i = 0; i < ctx.config.maxTargets; i++)
            {
                float along;
                EnemyController enemy = FindNext(ctx, origin, end, minAlong, false, out along);
                if (enemy == null)
                {
                    break;
                }

                minAlong = along;

                Target target = BuildTarget(enemy);
                ctx.hitPoint = target.position;
                RunHitModules(ctx, target);
            }

            IOnPostHit[] postHitModules = ctx.buildSnapshot.postHitModules;
            for (int i = 0; i < postHitModules.Length; i++)
            {
                postHitModules[i].OnPostHit(ctx);
            }
        }

        private static EnemyController FindNearest(ShotContext ctx)
        {
            float rangeSqr = ctx.config.range * ctx.config.range;
            EnemyController best = null;
            float bestSqr = float.PositiveInfinity;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

                float sqr = (enemy.transform.position - ctx.position).sqrMagnitude;
                if (sqr > rangeSqr || sqr >= bestSqr)
                {
                    continue;
                }

                bestSqr = sqr;
                best = enemy;
            }

            return best;
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
            target.faction = 0;
            target.enemy = enemy;

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
