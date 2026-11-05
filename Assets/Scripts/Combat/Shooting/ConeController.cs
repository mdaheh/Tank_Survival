using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class ConeController
    {
        private const float MinDirectionSqr = 1e-6f;

        private IVfxSpawner _vfx;

        private IVfxSpawner Vfx
        {
            get
            {
                if (_vfx == null)
                {
                    _vfx = GameServices.Get<IVfxSpawner>();
                }

                return _vfx;
            }
        }

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

            EnemyController nearest = FindNearest(ctx);
            if (nearest == null)
            {
                ctx.destroyReason = DestroyReason.External;
                DestroyShot(ctx);
                return;
            }

            SetAnchor(ctx, nearest);

            Vector3 toTarget = ctx.currentTarget.position - ctx.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > MinDirectionSqr)
            {
                ctx.direction = toTarget.normalized;
            }

            Vector3 origin = ctx.position;
            Vector3 flatDir = ctx.direction;
            flatDir.y = 0f;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowSector(origin, flatDir, ctx.config.angle, ctx.config.range, 0.25f,
                new Color(1f, 0.55f, 0.1f, 0.25f));
#endif

            if (ctx.age >= ctx.nextTickAt)
            {
                ctx.nextTickAt += ctx.config.tickRate;
                RunSectorHits(ctx, origin, flatDir);

                IOnPostHit[] postHitModules = ctx.buildSnapshot.postHitModules;
                for (int p = 0; p < postHitModules.Length; p++)
                {
                    postHitModules[p].OnPostHit(ctx);
                }

                IVfxSpawner vfx = Vfx;
                if (ctx.config.coneVfx != null && vfx != null)
                {
                    vfx.Spawn(ctx.config.coneVfx, ctx.position, ctx.direction);
                }
            }

            if (ctx.lifetime > 0f && ctx.age >= ctx.lifetime)
            {
                ctx.destroyReason = DestroyReason.Expired;
                DestroyShot(ctx);
            }
        }

        private static EnemyController FindNearest(ShotContext ctx)
        {
            EnemyController best = null;
            float bestSqr = float.PositiveInfinity;
            float rangeSqr = ctx.config.range * ctx.config.range;

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

        private static void SetAnchor(ShotContext ctx, EnemyController enemy)
        {
            ctx.currentTarget.position = enemy.transform.position;
            ctx.currentTarget.damageable = enemy.Damageable;
            ctx.currentTarget.faction = Faction.Enemy;
        }

        private static void RunSectorHits(ShotContext ctx, Vector3 origin, Vector3 flatDir)
        {
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
