using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Difficulty Data", fileName = "DifficultyData")]
    public class DifficultyData : ScriptableObject
    {
        public int waveCount = 3;
        public float countMultiplier = 1f;
        public float healthMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float xpMultiplier = 1f;
    }
}
