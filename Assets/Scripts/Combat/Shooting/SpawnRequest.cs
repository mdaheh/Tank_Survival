using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class SpawnRequest
    {
        public DeliveryType type;
        public Vector3 position;
        public Vector3 direction;
        public BuildSnapshot buildSnapshot;
        public DeliveryConfig config;
        public int depth;
    }
}
