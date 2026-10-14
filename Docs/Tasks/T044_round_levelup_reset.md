# T044: RoundController — фаза LevelUp + сброс раунда
- Status: [x]
- Phase: 6
- Depends: T039, T041
- Files:
  - Assets/Scripts/Round/RoundController.cs

## Задача
В `RoundController` (namespace `TankSurvival.Round`, добавить `using TankSurvival.Core;`, `using TankSurvival.Progression;`):
1. В `RoundPhase` добавить `LevelUp` (между `Playing` и `Win`).
2. Добавить сериализуемые поля `StatBlock statBlock`, `SessionState sessionState`, `ProgressionSystem progression`.
3. В `Awake` дополнительно подписаться: `progression.OnLevelUp += HandleLevelUp;` и `progression.OnLevelUpResolved += HandleLevelUpResolved;`.
4. `StartRound()`: перед `playerHealth.Init()` добавить `statBlock.Init(sessionState.SelectedChassis, sessionState.SelectedTurret);` и `progression.Reset();` (порядок: StatBlock → Progression → PlayerHealth → `waveSpawner.BeginWave`).
5. Добавить `private void HandleLevelUp() { phase = RoundPhase.LevelUp; }` и `private void HandleLevelUpResolved() { phase = RoundPhase.Playing; }`.

## Ограничения
- Без `SceneManager.LoadScene` (рестарт — сброс состояния).
- `HandleWaveCleared`/`HandlePlayerDeath`/`Restart` не менять.
- `phase`/`currentWave` остаются публичными полями.

## Unity (человеку)
- Объект `RoundController` → заполнить новые поля: `statBlock` → компонент `StatBlock` (на `Player`), `sessionState` → компонент `SessionState` (на `GameBootstrap`), `progression` → `ProgressionSystem`.

## Проверка человеком
- Инспектор `RoundController`: `statBlock`, `sessionState`, `progression` заполнены.
- Play Mode: при level-up `phase = LevelUp`; после выбора карточки `phase = Playing`.
- `Context Menu (⋮) → Restart Round`: `xp = 0`, `level = 1`, статы в `StatBlock` вернулись к базовым.

## Отчёт Исполнителя
Готово: Assets/Scripts/Round/RoundController.cs — фаза LevelUp в enum, поля statBlock/sessionState/progression, подписки на OnLevelUp/OnLevelUpResolved в Awake, StartRound (StatBlock → Progression.Reset → PlayerHealth → BeginWave), HandleLevelUp/HandleLevelUpResolved.
Блокер: —
