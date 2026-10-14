using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Turret Data")]
    public class TurretData : ScriptableObject
    {
        public DeliveryModuleData delivery;
        public OnHitModuleData[] onHitModules;
    }
}
