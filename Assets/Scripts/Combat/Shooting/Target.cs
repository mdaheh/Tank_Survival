using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public struct Target
    {
        public Vector3 position;
        public IDamageable damageable;
        public Faction faction;
    }
}
