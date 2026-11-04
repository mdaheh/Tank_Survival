using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class ConeController
    {
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

            Vector3 origin = ctx.position;
            Vector3 flatDir = ctx.direction;
            flatDir.y = 0f;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowSector(origin, flatDir, ctx.config.angle, ctx.config.range, 0.25f,
                new Color(1f, 0.55f, 0.1f, 0.25f));
#endif

            if (ctx.age >= ctx.lifetime)
            {
                ctx.destroyReason = DestroyReason.Expired;
                DestroyShot(ctx);
                return;
            }

            if (ctx.age < ctx.nextTickAt)
            {
                return;
            }

            ctx.nextTickAt = ctx.age + ctx.config.tickRate;

            float halfAngle = ctx.config.angle * 0.5f;
            float rangeSqr = ctx.config.range * ctx.config.range;

            for (int i = 0; i < EnemyController.Active.Count; i++)
            {
                EnemyController enemy = EnemyController.Active[i];
                if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
                {
                    continue;
                }

                Vector3 toEnemy = enemy.transform.position - origin;
                if (toEnemy.sqrMagnitude > rangeSqr)
                {
                    continue;
                }

                if (Vector3.Angle(flatDir, toEnemy) > halfAngle)
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
            target.faction = 0;

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
