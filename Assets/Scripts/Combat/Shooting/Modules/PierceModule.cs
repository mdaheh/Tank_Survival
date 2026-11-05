using TankSurvival.Data;

namespace TankSurvival.Combat.Shooting
{
    public class PierceModule : IOnSpawn, IOnPostHit
    {
        private readonly OnHitModuleData _data;

        public PierceModule(OnHitModuleData data)
        {
            _data = data;
        }

        public void OnSpawn(ShotContext ctx)
        {
            ctx.pierceRemaining = _data != null ? _data.maxPierceCount : -1;
        }

        public void OnPostHit(ShotContext ctx)
        {
            if (_data == null || ctx.pierceRemaining <= 0)
            {
                return;
            }

            ctx.pierceRemaining--;
            ctx.damage *= _data.damageFalloff;

            if (ctx.pierceRemaining > 0)
            {
                ctx.keepAlive = true;
            }
        }
    }
}
