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
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private RoundController roundController;

        private void Start()
        {
            poolManager.Prewarm(projectilePrefab, projectileCount);
            waveSpawner.Prewarm();
#if !UNITY_EDITOR && !DEBUG
            roundController.StartRound();
#endif
        }
    }
}
