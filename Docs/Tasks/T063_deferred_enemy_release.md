# T063: Смерть врага — постановка в очередь, а не немедленный Release
- Status: [ ]
- Phase: 9.5
- Depends: T062
- Files:
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Waves/WaveSpawner.cs

## Задача
Убрать немедленное освобождение врага при смерти. В `EnemyController.Init` расширить сигнатуру до `Init(EnemyData data, Transform target, PoolManager pool, EnemyKillSystem killSystem)` и заменить подписку `health.OnDeath += () => pool.Release(gameObject);` на `health.OnDeath += () => killSystem.Enqueue(gameObject);`. В `WaveSpawner` добавить поле `public EnemyKillSystem killSystem;` и передавать его в вызов `controller.Init(order.enemy, playerTransform, poolManager, killSystem);`.

## Ограничения
- `EnemyHealth.TakeDamage`/`OnDeath` не менять: смерть по-прежнему синхронно рассылает бухгалтерию (XP, счётчик волны) через `OnDeath`; меняется только точка `pool.Release`.
- Поле `_pool` в `EnemyController` оставить — оно используется для спавна вражеских снарядов; его использование не менять.
- Новое поле `_killSystem` в `EnemyController` не вводить: лямбда захватывает параметр `killSystem` напрямую.
- `WaveSpawner.BeginWave`/`ClearActiveEnemies` не менять (там `poolManager.Release` по живым врагам из `_active` — штатная очистка волны, не смерть).
- namespace/`using` сохранить; `EnemyKillSystem` — в том же `TankSurvival.Enemy`, дополнительный `using` не нужен.

## Unity (человеку)
- `WaveSpawner` → компонент `WaveSpawner` → поле `Kill System` → объект `EnemyKillSystem`.

## Проверка человеком
- Проект компилируется без ошибок.
- Инспектор `WaveSpawner`: поле `Kill System` ссылается на `EnemyKillSystem`.

## Отчёт Исполнителя
Готово:
Блокер:
