using TankSurvival.Data;
using TankSurvival.Player;
using TankSurvival.Waves;
using UnityEngine;

namespace TankSurvival.Round
{
    public class RoundController : MonoBehaviour
    {
        public enum RoundPhase
        {
            Playing,
            Win,
            Lose
        }

        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private DifficultyData difficulty;
        [SerializeField] private PlayerHealth playerHealth;

        public RoundPhase phase;
        public int currentWave;

        private void Awake()
        {
            playerHealth.OnDeath += HandlePlayerDeath;
            waveSpawner.OnWaveCleared += HandleWaveCleared;
        }

        public void StartRound()
        {
            currentWave = 1;
            phase = RoundPhase.Playing;
            playerHealth.Init();
            waveSpawner.BeginWave(currentWave);
        }

        private void HandleWaveCleared()
        {
            if (currentWave >= difficulty.waveCount)
            {
                phase = RoundPhase.Win;
            }
            else
            {
                currentWave++;
                waveSpawner.BeginWave(currentWave);
            }
        }

        private void HandlePlayerDeath()
        {
            phase = RoundPhase.Lose;
            waveSpawner.ClearActiveEnemies();
        }

        [ContextMenu("Restart Round")]
        public void Restart()
        {
            StartRound();
        }
    }
}
