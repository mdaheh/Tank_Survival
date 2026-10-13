using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Turret Data")]
    public class TurretData : ScriptableObject
    {
        public float aps;
        public float range;
        public float damage;
        public DeliveryModuleData delivery;
    }
}
