using System;
using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using TankSurvival.Progression;
using UnityEngine;

namespace TankSurvival.Waves
{
    public class WaveSpawner : MonoBehaviour
    {
        public WaveData waveData;
        public DifficultyData difficulty;
        public SpawnPoint[] spawnPoints;
        public PoolManager poolManager;
        public Transform playerTransform;
        public ProgressionSystem progression;

        public event Action OnWaveCleared;

        private readonly List<EnemyController> _active = new List<EnemyController>();
        private readonly List<SpawnOrder> _queue = new List<SpawnOrder>();
        private int _spawnIndex;
        private float _timer;
        private bool _spawning;

        private struct SpawnOrder
        {
            public EnemyData enemy;
            public GameObject prefab;
        }

        public void BeginWave(int waveNumber)
        {
            for (int i = 0; i < _queue.Count; i++)
            {
                _queue[i] = default;
            }

            _queue.Clear();

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                poolManager.Release(_active[i].gameObject);
            }

            _active.Clear();
            _spawnIndex = 0;

            for (int c = 0; c < waveData.compositions.Length; c++)
            {
                WaveComposition composition = waveData.compositions[c];
                int count = Mathf.RoundToInt(
                    (composition.baseCount + (waveNumber - 1) * composition.increase) * difficulty.countMultiplier);
                for (int i = 0; i < count; i++)
                {
                    _queue.Add(new SpawnOrder
                    {
                        enemy = composition.enemy,
                        prefab = composition.prefab
                    });
                }
            }

            _timer = 0f;
            _spawning = true;
        }

        private void Update()
        {
            if (!_spawning)
            {
                return;
            }

            if (_queue.Count > 0)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    SpawnNext();
                    _timer = waveData.spawnInterval;
                }
            }
        }

        private void SpawnNext()
        {
            SpawnOrder order = _queue[0];
            _queue.RemoveAt(0);

            GameObject go = poolManager.Get(order.prefab);
            go.transform.position = spawnPoints[_spawnIndex % spawnPoints.Length].transform.position;
            _spawnIndex++;

            EnemyController controller = go.GetComponent<EnemyController>();
            controller.health.healthMultiplier = difficulty.healthMultiplier;
            controller.speedMultiplier = difficulty.speedMultiplier;
            controller.health.OnDeath += () => { HandleEnemyDeath(controller); progression.AddXp(controller.data.xpReward * difficulty.xpMultiplier); };
            controller.Init(order.enemy, playerTransform, poolManager);
            _active.Add(controller);
        }

        private void HandleEnemyDeath(EnemyController controller)
        {
            _active.Remove(controller);
            if (_queue.Count == 0 && _active.Count == 0)
            {
                _spawning = false;
                OnWaveCleared?.Invoke();
            }
        }

        [ContextMenu("Release Active Enemies")]
        public void ClearActiveEnemies()
        {
            _spawning = false;
            _queue.Clear();
            _timer = 0f;

            List<EnemyController> copy = new List<EnemyController>(_active);
            for (int i = copy.Count - 1; i >= 0; i--)
            {
                poolManager.Release(copy[i].gameObject);
            }

            _active.Clear();
        }

        public void Prewarm()
        {
            for (int c = 0; c < waveData.compositions.Length; c++)
            {
                WaveComposition composition = waveData.compositions[c];
                int max = 0;
                for (int n = 1; n <= difficulty.waveCount; n++)
                {
                    int count = Mathf.RoundToInt(
                        (composition.baseCount + (n - 1) * composition.increase) * difficulty.countMultiplier);
                    if (count > max)
                    {
                        max = count;
                    }
                }

                poolManager.Prewarm(composition.prefab, max);
            }
        }

        [ContextMenu("Begin Wave 1")]
        public void BeginWave1()
        {
            BeginWave(1);
        }
    }
}
