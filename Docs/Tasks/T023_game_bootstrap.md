# T023: GameBootstrap — пересборка под волны и раунд
- Status: [ ]
- Phase: 3
- Depends: T021, T022
- Files:
  - Assets/Scripts/Core/GameBootstrap.cs

## Задача
Пересобрать `GameBootstrap` (namespace `TankSurvival.Core`) под WaveSpawner/RoundController.
Убрать временный спавн врагов: удалить сериализуемые поля `enemyPrefab`, `enemyData`,
`spawnPoints`, `spawnCount`, `playerHealth` и цикл спавна в `Start`. Оставить поля
`sessionState`, `statBlock`, `poolManager`, `projectilePrefab`, `projectileCount`; добавить
`[SerializeField] WaveSpawner waveSpawner`, `[SerializeField] RoundController roundController`.
`Start`: `statBlock.Init(sessionState.SelectedChassis); poolManager.Prewarm(projectilePrefab,
projectileCount); waveSpawner.Prewarm(); roundController.StartRound();`
(вызов `playerHealth.Init()` убрать — теперь это делает `RoundController.StartRound`).

## Ограничения
- Не добавлять `SceneManager.LoadScene`.
- `using TankSurvival.Waves` (WaveSpawner), `using TankSurvival.Round` (RoundController);
  убрать ставшие ненужными `using TankSurvival.Data`, `using TankSurvival.Enemy`,
  `using TankSurvival.Player`.
- Весь prewarm/спавн — только в `Start`, не в `Update`.

## Unity (человеку)
На GameObject `GameBootstrap` в `Assets/Scenes/Main.unity`:
1. Очистить (убрать ссылки) поля `enemyPrefab`, `enemyData`, `spawnPoints`, `spawnCount`, `playerHealth`.
2. Заполнить новые поля: `waveSpawner` → `WaveSpawner`, `roundController` → `RoundController`.
3. Убедиться, что `sessionState`/`statBlock`/`poolManager`/`projectilePrefab`/`projectileCount`
   подключены как раньше.

## Проверка человеком
- Проект компилируется без ошибок.
- Запустить Play Mode: раунд стартует через `RoundController` (см. Smoke-тест фазы 3).

## Отчёт Исполнителя
Готово:
Блокер:
