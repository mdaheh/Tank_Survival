using TankSurvival.Data;
using TankSurvival.Enemy;

namespace TankSurvival.Combat.Shooting
{
    public class LaserFocusModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        public LaserFocusModule(OnHitModuleData data)
        {
            _data = data;
        }

        public void OnHit(ShotContext ctx, Target target)
        {
            if (_data == null)
            {
                return;
            }

            if (!ctx.config.continuous)
            {
                return;
            }

            EnemyController enemy = target?.enemy;
            if (enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f)
            {
                return;
            }

            if (ctx.laserFocusedEnemy != enemy)
            {
                ctx.laserFocusedEnemy = enemy;

                if (_data.stackResetOnSwitch)
                {
                    ctx.laserStacks = 0;
                }
            }

            float stacks = ctx.laserStacks;
            if (stacks > 0f)
            {
                float extra = ctx.buildSnapshot.damage * _data.damageGrowth * stacks;
                if (extra > 0f)
                {
                    target.damageable.TakeDamage(extra);
                }
            }

            if (ctx.laserStacks < _data.maxStacks)
            {
                ctx.laserStacks++;
            }
        }
    }
}
