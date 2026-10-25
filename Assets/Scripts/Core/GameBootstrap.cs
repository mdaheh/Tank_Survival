using TankSurvival.Round;
using TankSurvival.Waves;
using UnityEngine;

namespace TankSurvival.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int projectileCount;
        [SerializeField] private GameObject enemyProjectilePrefab;
        [SerializeField] private int enemyProjectileCount;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private RoundController roundController;

        private void Start()
        {
            poolManager.Prewarm(projectilePrefab, projectileCount);
            poolManager.Prewarm(enemyProjectilePrefab, enemyProjectileCount);
            waveSpawner.Prewarm();
#if !UNITY_EDITOR && !DEBUG
            roundController.StartRound();
#endif
        }
    }
}
