using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Delivery Module Data")]
    public class DeliveryModuleData : ScriptableObject
    {
        public enum DeliveryKind
        {
            Projectile,
            Hitscan
        }

        public DeliveryKind kind;
        public float speed;
        public int pelletCount;
        public float spreadAngle;
    }
}
