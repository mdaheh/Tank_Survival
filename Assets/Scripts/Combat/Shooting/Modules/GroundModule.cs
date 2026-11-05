using TankSurvival.Data;

namespace TankSurvival.Combat.Shooting
{
    public class GroundModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        public GroundModule(OnHitModuleData data)
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

            BuildSnapshot child = ctx.buildSnapshot.Without<GroundModule>();
            child.damage = _data.tickDamage;

            DeliveryConfig cfg = default;
            cfg.range = _data.radius;
            cfg.tickRate = _data.tickRate;
            cfg.duration = _data.duration;

            SpawnRequest req = new SpawnRequest();
            req.type = DeliveryType.Zone;
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
