using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class ProjectileController
    {
        private const float HitRadiusSqr = 1f;

        public void Tick(ShotContext ctx, float dt)
        {
            ctx.age += dt;

            if (ctx.state == ShotState.Spawn)
            {
                ctx.state = ShotState.Flight;
                ctx.remainingRange = ctx.config.range;
                ctx.lifetime = ctx.buildSnapshot.speed > 0f
                    ? ctx.config.range / ctx.buildSnapshot.speed
                    : -1f;
            }

            if (ctx.state != ShotState.Flight)
            {
                return;
            }

            IOnFlight[] flightModules = ctx.buildSnapshot.flightModules;
            for (int i = 0; i < flightModules.Length; i++)
            {
                flightModules[i].OnFlight(ctx, dt);
            }

            Vector3 velocity = ctx.direction * ctx.buildSnapshot.speed;
            ctx.prevPosition = ctx.position;
            ctx.position += velocity * dt;
            ctx.remainingRange -= velocity.magnitude * dt;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowBeam(ctx.prevPosition, ctx.position, Time.deltaTime, new Color(1f, 0.5f, 0f, 0.5f));
#endif

            if (ctx.remainingRange <= 0f || (ctx.lifetime > 0f && ctx.age >= ctx.lifetime))
            {
                ctx.destroyReason = DestroyReason.Expired;
                DestroyShot(ctx);
                return;
            }

            bool hit = ctx.faction == Faction.Enemy ? HitPlayer(ctx) : HitEnemies(ctx);

            if (!hit)
            {
                return;
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

        private static bool HitEnemies(ShotContext ctx)
        {
            bool hit = false;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

                if (IsAlreadyHit(ctx, enemy.Damageable))
                {
                    continue;
                }

                Vector3 hitPoint;
                if (!TrySweepPoint(ctx.prevPosition, ctx.position, enemy.transform.position, out hitPoint))
                {
                    continue;
                }

                hit = true;

                Target target = new Target();
                target.position = enemy.transform.position;
                target.damageable = enemy.Damageable;
                target.faction = Faction.Enemy;
                target.enemy = enemy;
                ctx.hitTargets.Add(target);

                ctx.hitPoint = hitPoint;

                IOnHit[] hitModules = ctx.buildSnapshot.hitModules;
                for (int m = 0; m < hitModules.Length; m++)
                {
                    hitModules[m].OnHit(ctx, target);
                }
            }

            return hit;
        }

        private static bool HitPlayer(ShotContext ctx)
        {
            if (!Combatants.TryGetPlayer(out Vector3 playerPosition, out IDamageable player))
            {
                return false;
            }

            if (IsAlreadyHit(ctx, player))
            {
                return false;
            }

            Vector3 hitPoint;
            if (!TrySweepPoint(ctx.prevPosition, ctx.position, playerPosition, out hitPoint))
            {
                return false;
            }

            Target target = new Target();
            target.position = playerPosition;
            target.damageable = player;
            target.faction = Faction.Player;
            target.enemy = null;
            ctx.hitTargets.Add(target);

            ctx.hitPoint = hitPoint;

            IOnHit[] hitModules = ctx.buildSnapshot.hitModules;
            for (int m = 0; m < hitModules.Length; m++)
            {
                hitModules[m].OnHit(ctx, target);
            }

            return true;
        }

        private static bool IsAlreadyHit(ShotContext ctx, IDamageable damageable)
        {
            for (int i = 0; i < ctx.hitTargets.Count; i++)
            {
                if (ctx.hitTargets[i].damageable == damageable)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TrySweepPoint(Vector3 from, Vector3 to, Vector3 point, out Vector3 hitPoint)
        {
            Vector3 segment = to - from;
            float lengthSqr = segment.sqrMagnitude;
            float t = lengthSqr > 0f ? Mathf.Clamp01(Vector3.Dot(point - from, segment) / lengthSqr) : 0f;

            hitPoint = from + segment * t;

            return (point - hitPoint).sqrMagnitude <= HitRadiusSqr;
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
