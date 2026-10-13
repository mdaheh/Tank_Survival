# T022: RoundController — фазы, победа/поражение, рестарт
- Status: [x]
- Phase: 3
- Depends: T019, T021
- Files:
  - Assets/Scripts/Round/RoundController.cs

## Задача
Создать `RoundController` (MonoBehaviour, namespace `TankSurvival.Round`).
Поля: `[SerializeField] WaveSpawner waveSpawner`, `[SerializeField] DifficultyData difficulty`,
`[SerializeField] PlayerHealth playerHealth`; `public enum RoundPhase { Playing, Win, Lose }`;
`public RoundPhase phase`, `public int currentWave`.
В `Awake` подписаться: `playerHealth.OnDeath += HandlePlayerDeath`,
`waveSpawner.OnWaveCleared += HandleWaveCleared`.
Методы:
- `public void StartRound()`: `currentWave = 1; phase = RoundPhase.Playing; playerHealth.Init();
  waveSpawner.BeginWave(currentWave);`
- `private void HandleWaveCleared()`: если `currentWave >= difficulty.waveCount` →
  `phase = RoundPhase.Win`; иначе `currentWave++; waveSpawner.BeginWave(currentWave);`
- `private void HandlePlayerDeath()`: `phase = RoundPhase.Lose; waveSpawner.Reset();`
- `[ContextMenu("Restart Round")] public void Restart()` → `StartRound()`.

## Ограничения
- Без `SceneManager.LoadScene` (рестарт — сброс состояния, не перезагрузка сцены).
- `phase` и `currentWave` — публичные поля (для наблюдения в инспекторе), не свойства.
- namespace `TankSurvival.Round`; `using TankSurvival.Player` (PlayerHealth),
  `using TankSurvival.Waves` (WaveSpawner), `using TankSurvival.Data` (DifficultyData).

## Unity (человеку)
В `Assets/Scenes/Main.unity`:
1. Создать пустой GameObject `RoundController`; добавить компонент `RoundController`.
2. Заполнить поля: `waveSpawner` → `WaveSpawner`, `difficulty` → `Sergeant.asset`,
   `playerHealth` → игрок (объект с `PlayerHealth`).

## Проверка человеком
- Запустить Play Mode: стартует волна 1 (`currentWave = 1`), враги спавнятся.
- После зачистки 3-й волны `phase = Win` (видно в инспекторе `RoundController`).
- При смерти игрока `phase = Lose`, враги возвращаются в пул.
- `Context Menu (⋮) → Restart Round` на компоненте `RoundController` — раунд стартует заново
  (`phase = Playing`, `currentWave = 1`) без перезагрузки сцены.

## Отчёт Исполнителя
Готово: Assets/Scripts/Round/RoundController.cs: RoundController с фазами, StartRound/HandleWaveCleared/HandlePlayerDeath/Restart
Блокер: —
