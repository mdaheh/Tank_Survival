using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Upgrade Data", fileName = "UpgradeData")]
    public class UpgradeData : ScriptableObject
    {
        public enum StatType { Damage, FireRate, MoveSpeed, MaxHealth }

        public StatType stat;
        public float value;
        public int maxStacks;
    }
}
