using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif

namespace TankSurvival.Combat.Shooting
{
    public class AoEController
    {
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
            ctx.hitPoint = ctx.position;

            float rangeSqr = ctx.config.range * ctx.config.range;

#if UNITY_EDITOR || DEBUG
            DevVisualizer.ShowCircle(ctx.position, ctx.config.range, 0.3f, new Color(1f, 0.25f, 0.2f, 0.5f));
#endif

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

                Target target = BuildTarget(enemy);
                ctx.hitTargets.Add(target);
                RunHitModules(ctx, target);
            }

            ctx.keepAlive = false;

            IOnPostHit[] postHitModules = ctx.buildSnapshot.postHitModules;
            for (int i = 0; i < postHitModules.Length; i++)
            {
                postHitModules[i].OnPostHit(ctx);
            }

            ctx.destroyReason = DestroyReason.Hit;
            DestroyShot(ctx);
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

        private void DestroyShot(ShotContext ctx)
        {
            IOnDestroy[] destroyModules = ctx.buildSnapshot.destroyModules;
            for (int i = 0; i < destroyModules.Length; i++)
            {
                destroyModules[i].OnDestroy(ctx);
            }

            IVfxSpawner vfx = Vfx;
            if (ctx.config.deathVfx != null && vfx != null)
            {
                vfx.Spawn(ctx.config.deathVfx, ctx.position);
            }

            ctx.state = ShotState.Dead;
        }
    }
}
