using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class SlowOnHitHandler : MonoBehaviour, IHitHandler
    {
        public ControlEffectSystem controlEffects;

        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || controlEffects == null || target == null)
            {
                return;
            }

            controlEffects.ApplySlow(target, module.slowAmount, module.duration);
        }
    }
}
