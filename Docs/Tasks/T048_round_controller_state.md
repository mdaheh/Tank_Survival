# T048: RoundController/WaveSpawner — сложность из SessionState + kills + события
- Status: [x]
- Phase: 7
- Depends: T047
- Files:
  - Assets/Scripts/Round/RoundController.cs
  - Assets/Scripts/Waves/WaveSpawner.cs

## Задача
1. `WaveSpawner` (namespace `TankSurvival.Waves`): добавить `public event Action OnEnemyKilled;` рядом с `OnWaveCleared`; в `HandleEnemyDeath` первой строкой вызвать `OnEnemyKilled?.Invoke();`.
2. `RoundController` (namespace `TankSurvival.Round`):
   - `using`: добавить `using System;`, убрать `using TankSurvival.Data;` (тип `DifficultyData` больше не упоминается).
   - убрать поле `[SerializeField] private DifficultyData difficulty;`.
   - добавить `public int kills;` и свойство `public int TotalWaves => sessionState.SelectedDifficulty.waveCount;`.
   - добавить события `public event Action<RoundPhase> OnPhaseChanged;`, `public event Action OnWaveChanged;`, `public event Action OnKillsChanged;`.
   - `Awake`: первой строкой добавить `waveSpawner.difficulty = sessionState.SelectedDifficulty;` (чтобы `Prewarm()` в `GameBootstrap.Start` видел выбранную сложность); подписаться `waveSpawner.OnEnemyKilled += HandleEnemyKilled;`.
   - `StartRound()`: установить `currentWave = 1; kills = 0;`, заново `waveSpawner.difficulty = sessionState.SelectedDifficulty;`, заменить `phase = RoundPhase.Playing;` на `SetPhase(RoundPhase.Playing);`, после `playerHealth.Init()` добавить `OnWaveChanged?.Invoke(); OnKillsChanged?.Invoke();` (порядок: сложность → SetPhase → StatBlock → Progression → PlayerHealth → события → `BeginWave`).
   - `HandleWaveCleared()`: условие `currentWave >= difficulty.waveCount` заменить на `currentWave >= sessionState.SelectedDifficulty.waveCount`; `phase = RoundPhase.Win;` → `SetPhase(RoundPhase.Win);`; в ветке else после `currentWave++;` добавить `OnWaveChanged?.Invoke();`.
   - `HandlePlayerDeath()`: `phase = RoundPhase.Lose;` → `SetPhase(RoundPhase.Lose);`.
   - `HandleLevelUp()`: `phase = RoundPhase.LevelUp;` → `SetPhase(RoundPhase.LevelUp);`.
   - `HandleLevelUpResolved()`: `phase = RoundPhase.Playing;` → `SetPhase(RoundPhase.Playing);`.
   - добавить методы:
     `private void HandleEnemyKilled() { kills++; OnKillsChanged?.Invoke(); }`
     `private void SetPhase(RoundPhase value) { phase = value; OnPhaseChanged?.Invoke(phase); }`
   - `Restart()` не менять (вызывает `StartRound()`).

## Ограничения
- Без `SceneManager.LoadScene`.
- `phase`/`currentWave`/`kills` — публичные поля (для наблюдения в инспекторе).
- `ClearActiveEnemies()` не вызывает `OnEnemyKilled` (освобождение через пул не считается убийством) — не трогать.
- `waveSpawner.difficulty` теперь единственный runtime-источник множителей для `WaveSpawner`.

## Unity (человеку)
- Компонент `RoundController`: поле `difficulty` исчезнет — это ожидаемо, ничего не дозаполнять; `sessionState` должен быть заполнен (T047).
- Компонент `WaveSpawner`: поле `difficulty` перезаписывается в `RoundController.Awake` из `SessionState` — можно оставить прежнее значение или очистить.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Инспектор `RoundController` в Play Mode: `kills` = 0 в начале, растёт на +1 за каждого убитого врага; `phase` переходит `Playing → Win/Lose` корректно; `currentWave` растёт после зачистки.
- `Context Menu (⋮) → Restart Round`: `kills` = 0, `currentWave` = 1.
- Смена `SessionState.selectedDifficulty` (Recruit ↔ Sergeant) меняет `waveCount` — проверяется через HUD в T050.

## Отчёт Исполнителя
Готово: Assets/Scripts/Waves/WaveSpawner.cs — событие `OnEnemyKilled` + вызов первой строкой в `HandleEnemyDeath`; Assets/Scripts/Round/RoundController.cs — сложность из `SessionState` (поле `difficulty` убрано), `kills`/`TotalWaves`, события `OnPhaseChanged`/`OnWaveChanged`/`OnKillsChanged`, `SetPhase`/`HandleEnemyKilled`, обновлённые `Awake`/`StartRound`/обработчики фаз.
Блокер: —
