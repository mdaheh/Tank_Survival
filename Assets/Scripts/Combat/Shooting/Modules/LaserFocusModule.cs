using TankSurvival.Core;
using TankSurvival.Data;

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

            IDamageable damageable = target.damageable;
            if (damageable == null || !damageable.IsAlive)
            {
                return;
            }

            if (ctx.laserFocusedTarget != damageable)
            {
                ctx.laserFocusedTarget = damageable;

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
                    damageable.TakeDamage(extra);
                }
            }

            if (ctx.laserStacks < _data.maxStacks)
            {
                ctx.laserStacks++;
            }
        }
    }
}
