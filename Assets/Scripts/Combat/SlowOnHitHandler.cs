using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class SlowOnHitHandler : MonoBehaviour, IHitHandler
    {
        private ControlEffectSystem _controlEffects;

        private ControlEffectSystem ControlEffects =>
            _controlEffects != null ? _controlEffects : (_controlEffects = GameServices.Get<ControlEffectSystem>());

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || ControlEffects == null || target == null)
            {
                return;
            }

            ControlEffects.ApplySlow(target, module.slowAmount, module.duration);
        }
    }
}
