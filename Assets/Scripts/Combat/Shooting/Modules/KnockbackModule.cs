using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class KnockbackModule : IOnHit
    {
        private readonly OnHitModuleData _data;

        private ControlEffectSystem _controlEffects;

        public KnockbackModule(OnHitModuleData data)
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
            if (damageable == null || !damageable.IsAlive)
            {
                return;
            }

            Vector3 dir = target.position - ctx.hitPoint;
            dir.y = 0f;
            dir.Normalize();

            ControlEffectSystem controlEffects = ControlEffects;
            if (controlEffects != null)
            {
                controlEffects.ApplyKnockback(damageable, dir * (_data.force * _data.damageScaling), _data.duration);
            }
        }
    }
}
