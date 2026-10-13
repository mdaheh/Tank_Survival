# T014: GameBootstrap — prewarm пулов и временный спавн
- Status: [ ]
- Phase: 2
- Priority: High
- Depends: T010, T011, T013
- Files:
  - Assets/Scripts/Core/GameBootstrap.cs

## Задача
Расширить `GameBootstrap` (namespace `TankSurvival.Core`): добавить сериализуемые поля
`PoolManager poolManager`, `GameObject enemyPrefab`, `EnemyData enemyData`,
`Transform playerTransform`, `Transform[] spawnPoints`, `int spawnCount`,
`GameObject projectilePrefab`, `int projectileCount`. В `Start` после текущей инициализации
StatBlock/PlayerHealth: `poolManager.Prewarm(enemyPrefab, spawnCount)` и
`poolManager.Prewarm(projectilePrefab, projectileCount)`; затем `spawnCount` раз взять
`poolManager.Get(enemyPrefab)`, выставить позицию `spawnPoints[i % spawnPoints.Length].position`,
вызвать `enemy.GetComponent<EnemyController>().Init(enemyData, playerTransform, poolManager)`.

## Ограничения
- Спавн и prewarm — только в `Start` (событие), не в `Update`.
- Существующую логику `Start` (init StatBlock/PlayerHealth) сохранить.
- namespace `TankSurvival.Core`; `using TankSurvival.Data` (EnemyData), `using TankSurvival.Enemy` (EnemyController).

## Unity (человеку)
В существующей сцене `Assets/Scenes/Main.unity`:
1. Создать пустой GameObject `PoolManager` → добавить компонент `PoolManager`.
2. Создать 4 пустых объекта `SpawnPoint_1..4` по краям карты.
3. На GameObject `GameBootstrap` заполнить новые поля: `poolManager` → `PoolManager`,
   `enemyPrefab` → `Runner.prefab`, `enemyData` → `Runner.asset`, `playerTransform` → игрок,
   `spawnPoints` → 4 SpawnPoint, `spawnCount` → 5, `projectilePrefab` → `Projectile.prefab`,
   `projectileCount` → 20.
4. Дособрать турель из T013: `ProjectileDeliveryHandler.pool` → `PoolManager`.
5. Убедиться, что `sessionState`/`statBlock`/`playerHealth` из фазы 1 подключены как раньше.

## Проверка человеком
- Запустить Play Mode (см. Smoke-тест фазы 2).

## Отчёт Исполнителя
Готово:
Блокер:
