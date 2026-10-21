using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class FreezeOnHitHandler : MonoBehaviour, IHitHandler
    {
        public ControlEffectSystem controlEffects;

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || controlEffects == null || target == null)
            {
                return;
            }

            controlEffects.ApplyFreeze(target, module.slowAmount, module.duration, module.freezeThreshold);
        }
    }
}
