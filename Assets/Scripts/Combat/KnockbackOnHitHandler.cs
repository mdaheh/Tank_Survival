using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class KnockbackOnHitHandler : MonoBehaviour, IHitHandler
    {
        public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)
        {
            if (module == null || target == null || target.health == null || target.health.currentHealth <= 0f)
            {
                return;
            }

            Vector3 dir = target.transform.position - position;
            dir.y = 0f;
            dir.Normalize();

            target.ApplyKnockback(dir * (module.force * module.damageScaling), module.duration);
        }
    }
}
