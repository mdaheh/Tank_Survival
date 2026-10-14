using System;
using TankSurvival.Core;
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
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private StatBlock statBlock;
        [SerializeField] private SessionState sessionState;
        [SerializeField] private ProgressionSystem progression;

        public RoundPhase phase;
        public int currentWave;
        public int kills;

        public int TotalWaves => sessionState.SelectedDifficulty.waveCount;

        public event Action<RoundPhase> OnPhaseChanged;
        public event Action OnWaveChanged;
        public event Action OnKillsChanged;

        private void Awake()
        {
            waveSpawner.difficulty = sessionState.SelectedDifficulty;
            playerHealth.OnDeath += HandlePlayerDeath;
            waveSpawner.OnWaveCleared += HandleWaveCleared;
            waveSpawner.OnEnemyKilled += HandleEnemyKilled;
            progression.OnLevelUp += HandleLevelUp;
            progression.OnLevelUpResolved += HandleLevelUpResolved;
        }

        public void StartRound()
        {
            currentWave = 1;
            kills = 0;
            waveSpawner.difficulty = sessionState.SelectedDifficulty;
            SetPhase(RoundPhase.Playing);
            statBlock.Init(sessionState.SelectedChassis, sessionState.SelectedTurret);
            progression.Reset();
            playerHealth.Init();
            OnWaveChanged?.Invoke();
            OnKillsChanged?.Invoke();
            waveSpawner.BeginWave(currentWave);
        }

        private void HandleWaveCleared()
        {
            if (currentWave >= sessionState.SelectedDifficulty.waveCount)
            {
                SetPhase(RoundPhase.Win);
            }
            else
            {
                currentWave++;
                OnWaveChanged?.Invoke();
                waveSpawner.BeginWave(currentWave);
            }
        }

        private void HandlePlayerDeath()
        {
            SetPhase(RoundPhase.Lose);
            waveSpawner.ClearActiveEnemies();
        }

        private void HandleLevelUp()
        {
            SetPhase(RoundPhase.LevelUp);
        }

        private void HandleLevelUpResolved()
        {
            SetPhase(RoundPhase.Playing);
        }

        private void HandleEnemyKilled()
        {
            kills++;
            OnKillsChanged?.Invoke();
        }

        private void SetPhase(RoundPhase value)
        {
            phase = value;
            OnPhaseChanged?.Invoke(phase);
        }

        [ContextMenu("Restart Round")]
        public void Restart()
        {
            StartRound();
        }
    }
}
