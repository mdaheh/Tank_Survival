using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class Target
    {
        public Vector3 position;
        public IDamageable damageable;
        public int faction;
    }
}
