using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class ZoneController
    {
        public void Tick(ShotContext ctx, float dt)
        {
            ctx.age += dt;

            if (ctx.state == ShotState.Spawn)
            {
                ctx.state = ShotState.Flight;
                ctx.lifetime = ctx.config.duration;
            }

            if (ctx.state != ShotState.Flight)
            {
                return;
            }

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowCircle(ctx.position, ctx.config.range, Mathf.Max(0.1f, ctx.lifetime - ctx.age),
                new Color(0.4f, 0.9f, 0.4f, 0.35f));
#endif

            if (ctx.lifetime > 0f && ctx.age >= ctx.lifetime)
            {
                ctx.destroyReason = DestroyReason.Expired;
                DestroyShot(ctx);
                return;
            }

            if (ctx.age >= ctx.nextTickAt)
            {
                ctx.nextTickAt += ctx.config.tickRate;
                RunAreaHits(ctx);

                IOnPostHit[] postHitModules = ctx.buildSnapshot.postHitModules;
                for (int i = 0; i < postHitModules.Length; i++)
                {
                    postHitModules[i].OnPostHit(ctx);
                }
            }
        }

        private static void RunAreaHits(ShotContext ctx)
        {
            float rangeSqr = ctx.config.range * ctx.config.range;
            ctx.hitPoint = ctx.position;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

                if ((enemy.transform.position - ctx.position).sqrMagnitude > rangeSqr)
                {
                    continue;
                }

                RunHitModules(ctx, BuildTarget(enemy));
            }
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
