using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Data;

namespace TankSurvival.Combat.Shooting
{
    public class SlowModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        private ControlEffectSystem _controlEffects;

        public SlowModule(OnHitModuleData data)
        {
            _data = data;
        }

        private ControlEffectSystem ControlEffects
        {
            get
            {
                if (_controlEffects == null)
                {
                    _controlEffects = GameServices.Get<ControlEffectSystem>();
                }

                return _controlEffects;
            }
        }

        public void OnHit(ShotContext ctx, Target target)
        {
            IDamageable damageable = target.damageable;
            if (damageable == null)
            {
                return;
            }

            ControlEffectSystem controlEffects = ControlEffects;
            if (controlEffects != null)
            {
                controlEffects.ApplySlow(damageable, _data.slowAmount, _data.duration);
            }
        }
    }
}
