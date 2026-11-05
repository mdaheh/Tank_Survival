namespace TankSurvival.Combat.Shooting
{
    public class DamageModule : IOnHit
    {
        public void OnHit(ShotContext ctx, Target target)
        {
            if (target == null || target.damageable == null)
            {
                return;
            }

            target.damageable.TakeDamage(ctx.damage);
        }
    }
}
