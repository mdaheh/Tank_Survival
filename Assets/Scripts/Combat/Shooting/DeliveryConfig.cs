using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public struct DeliveryConfig
    {
        public float range;
        public float angle;
        public int maxTargets;
        public bool continuous;
        public float tickRate;
        public GameObject coneVfx;
    }
}
