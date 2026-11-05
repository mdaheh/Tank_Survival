using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;

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
            EnemyController enemy = target?.enemy;
            if (enemy == null)
            {
                return;
            }

            ControlEffectSystem controlEffects = ControlEffects;
            if (controlEffects != null)
            {
                controlEffects.ApplySlow(enemy, _data.slowAmount, _data.duration);
            }
        }
    }
}
