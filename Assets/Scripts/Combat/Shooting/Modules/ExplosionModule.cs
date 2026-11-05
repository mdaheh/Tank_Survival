using TankSurvival.Data;

namespace TankSurvival.Combat.Shooting
{
    public class ExplosionModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        public ExplosionModule(OnHitModuleData data)
        {
            _data = data;
        }

        public void OnHit(ShotContext ctx, Target target)
        {
            if (_data == null)
            {
                return;
            }

            if (ctx.depth >= _data.maxDepth)
            {
                return;
            }

            BuildSnapshot child = ctx.buildSnapshot.Without<ExplosionModule>();
            child.damage = _data.damage;

            DeliveryConfig cfg = default;
            cfg.range = _data.radius;
            cfg.deathVfx = _data.explosionVfx;

            SpawnRequest req = new SpawnRequest();
            req.type = DeliveryType.AoE;
            req.position = ctx.hitPoint;
            req.direction = ctx.direction;
            req.buildSnapshot = child;
            req.config = cfg;
            req.depth = ctx.depth + 1;
            req.faction = ctx.faction;

            ctx.spawnQueue.Add(req);
        }
    }
}
