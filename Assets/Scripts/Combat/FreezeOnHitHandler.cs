using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class FreezeOnHitHandler : MonoBehaviour, IHitHandler
    {
        private ControlEffectSystem _controlEffects;
        private StatusEffectSystem _statusEffects;

        private ControlEffectSystem ControlEffects =>
            _controlEffects != null ? _controlEffects : (_controlEffects = GameServices.Get<ControlEffectSystem>());

        private StatusEffectSystem StatusEffects =>
            _statusEffects != null ? _statusEffects : (_statusEffects = GameServices.Get<StatusEffectSystem>());

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || target == null)
            {
                return;
            }

            if (StatusEffects != null)
            {
                StatusEffects.RemoveBurn(target);
            }

            if (ControlEffects == null)
            {
                return;
            }

            ControlEffects.ApplyFreeze(target, module.slowAmount, module.duration, module.freezeThreshold);
        }
    }
}
