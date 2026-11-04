using TankSurvival.Core;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class Target
    {
        public Vector3 position;
        public IDamageable damageable;
        public EnemyController enemy;
        public int faction;
    }
}
