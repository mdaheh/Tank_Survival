# T055: GameBootstrap — prewarm вражеских снарядов
- Status: [ ]
- Phase: 8
- Depends: T053
- Files:
  - Assets/Scripts/Core/GameBootstrap.cs

## Задача
В `GameBootstrap` (namespace `TankSurvival.Core`) добавить prewarm пула
вражеских снарядов рядом с `projectilePrefab`:
- `[SerializeField] private GameObject enemyProjectilePrefab;`
- `[SerializeField] private int enemyProjectileCount;`
- в `Start()` после `poolManager.Prewarm(projectilePrefab, projectileCount);`
  добавить `poolManager.Prewarm(enemyProjectilePrefab, enemyProjectileCount);`.

## Ограничения
- Порядок существующих вызовов `Start()` не менять; только добавить prewarm
  вражеских снарядов.

## Unity (человеку)
1. GameObject `GameBootstrap` → компонент `GameBootstrap` →
   `enemyProjectilePrefab` → `Assets/Prefabs/Enemies/EnemyProjectile.prefab`;
   `enemyProjectileCount` → 20.

## Проверка человеком
- Инспектор `GameBootstrap`: `enemyProjectilePrefab` и `enemyProjectileCount`
  заполнены.
- Play Mode: до старта волны в иерархии под `PoolManager` создано 20
  неактивных `EnemyProjectile` (как для `projectilePrefab`).
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
