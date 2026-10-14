using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public interface IHitHandler
    {
        void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target);
    }
}