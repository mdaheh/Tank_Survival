using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public float health;
        public float speed;
        public float damage;
        public int xpReward;
        public string behavior = "melee";
    }
}
