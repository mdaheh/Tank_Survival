using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Enemy;

namespace TankSurvival.Combat.Shooting
{
    public class LegacyOnHitModule : IOnHit
    {
        private OnHitDispatcher dispatcher;

        public void OnHit(ShotContext ctx, Target target)
        {
            if (dispatcher == null)
            {
                dispatcher = GameServices.Get<OnHitDispatcher>();
                if (dispatcher == null) return;
            }

            EnemyController enemy = target?.enemy;
            if (enemy == null) return;

            dispatcher.ProcessHit(ctx.hitPoint, enemy, ctx.depth);
        }
    }
}
