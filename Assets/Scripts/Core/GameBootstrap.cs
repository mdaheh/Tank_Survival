using TankSurvival.Data;
using TankSurvival.Enemy;
using TankSurvival.Player;
using UnityEngine;

namespace TankSurvival.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private SessionState sessionState;
        [SerializeField] private StatBlock statBlock;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int spawnCount;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int projectileCount;

        private void Start()
        {
            statBlock.Init(sessionState.SelectedChassis);
            playerHealth.Init();

            poolManager.Prewarm(enemyPrefab, spawnCount);
            poolManager.Prewarm(projectilePrefab, projectileCount);

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = poolManager.Get(enemyPrefab);
                enemy.transform.position = spawnPoints[i % spawnPoints.Length].position;
                enemy.GetComponent<EnemyController>().Init(enemyData, playerTransform, poolManager);
            }
        }
    }
}
