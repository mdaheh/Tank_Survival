using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;

namespace TankSurvival.Combat.Shooting
{
    public class BurnModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        private ControlEffectSystem _controlEffects;
        private StatusEffectSystem _statusEffects;

        public BurnModule(OnHitModuleData data)
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

        private StatusEffectSystem StatusEffects
        {
            get
            {
                if (_statusEffects == null)
                {
                    _statusEffects = GameServices.Get<StatusEffectSystem>();
                }

                return _statusEffects;
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
                controlEffects.RemoveFreeze(enemy);
            }

            StatusEffectSystem statusEffects = StatusEffects;
            if (statusEffects != null)
            {
                statusEffects.ApplyBurn(enemy, _data.tickDamage, _data.tickRate, _data.duration);
            }
        }
    }
}
