using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class BurnOnHitHandler : MonoBehaviour, IHitHandler
    {
        public StatusEffectSystem statusEffects;
        public ControlEffectSystem controlEffects;

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || target == null)
            {
                return;
            }

            if (controlEffects != null)
            {
                controlEffects.RemoveFreeze(target);
            }

            if (statusEffects == null)
            {
                return;
            }

            statusEffects.ApplyBurn(target, module.tickDamage, module.tickRate, module.duration);
        }
    }
}