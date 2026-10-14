using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Player;
using TankSurvival.Progression;
using TankSurvival.Waves;
using UnityEngine;

namespace TankSurvival.Round
{
    public class RoundController : MonoBehaviour
    {
        public enum RoundPhase
        {
            Playing,
            LevelUp,
            Win,
            Lose
        }

        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private DifficultyData difficulty;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private StatBlock statBlock;
        [SerializeField] private SessionState sessionState;
        [SerializeField] private ProgressionSystem progression;

        public RoundPhase phase;
        public int currentWave;

        private void Awake()
        {
            playerHealth.OnDeath += HandlePlayerDeath;
            waveSpawner.OnWaveCleared += HandleWaveCleared;
            progression.OnLevelUp += HandleLevelUp;
            progression.OnLevelUpResolved += HandleLevelUpResolved;
        }

        public void StartRound()
        {
            currentWave = 1;
            phase = RoundPhase.Playing;
            statBlock.Init(sessionState.SelectedChassis, sessionState.SelectedTurret);
            progression.Reset();
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

        private void HandleLevelUp()
        {
            phase = RoundPhase.LevelUp;
        }

        private void HandleLevelUpResolved()
        {
            phase = RoundPhase.Playing;
        }

        [ContextMenu("Restart Round")]
        public void Restart()
        {
            StartRound();
        }
    }
}
