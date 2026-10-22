using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class BurnOnHitHandler : MonoBehaviour, IHitHandler
    {
        private StatusEffectSystem _statusEffects;
        private ControlEffectSystem _controlEffects;

        private StatusEffectSystem StatusEffects =>
            _statusEffects != null ? _statusEffects : (_statusEffects = GameServices.Get<StatusEffectSystem>());

        private ControlEffectSystem ControlEffects =>
            _controlEffects != null ? _controlEffects : (_controlEffects = GameServices.Get<ControlEffectSystem>());

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || target == null)
            {
                return;
            }

            if (ControlEffects != null)
            {
                ControlEffects.RemoveFreeze(target);
            }

            if (StatusEffects == null)
            {
                return;
            }

            StatusEffects.ApplyBurn(target, module.tickDamage, module.tickRate, module.duration);
        }
    }
}