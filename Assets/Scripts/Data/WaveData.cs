using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/Wave Data", fileName = "WaveData")]
    public class WaveData : ScriptableObject
    {
        public float spawnInterval = 2f;
        public WaveComposition[] compositions;
    }

    [System.Serializable]
    public struct WaveComposition
    {
        public EnemyData enemy;
        public GameObject prefab;
        public int baseCount;
        public int increase;
    }
}
