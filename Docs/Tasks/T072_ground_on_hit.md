# T072: GroundOnHitHandler + GroundZone + роутинг Ground
- Status: [ ]
- Phase: 10
- Depends: T066, T075
- Files:
  - Assets/Scripts/Combat/GroundOnHitHandler.cs
  - Assets/Scripts/Combat/GroundZone.cs
  - Assets/Scripts/Combat/OnHitDispatcher.cs

## Задача
1) `GroundOnHitHandler : MonoBehaviour, IHitHandler` (namespace `TankSurvival.Combat`): поля `public PoolManager pool`, `public OnHitDispatcher onHit`. Реализовать `HandleHit(module, position, target)` → делегирует в `HandleHit(module, position, target, 0)`. `public void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target, int depth)`: если `module == null || module.groundVfx == null || pool == null || onHit == null` — выйти; `GameObject zone = pool.Get(module.groundVfx); zone.transform.position = position; zone.GetComponent<GroundZone>().Init(module, position, pool, onHit, depth);`.
2) `GroundZone : MonoBehaviour, IPoolable` (namespace `TankSurvival.Combat`): приватные `PoolManager _pool`, `OnHitDispatcher _onHit`, `float _radius`, `float _tickDamage`, `float _tickRate`, `float _duration`, `int _depth`, `float _elapsed`, `float _tickTimer`. `Init(OnHitModuleData module, Vector3 position, PoolManager pool, OnHitDispatcher onHit, int depth)` заполняет поля из `module`, ставит `transform.position = position`, сбрасывает таймеры. В `Update`: если `_pool == null` — выйти; `_elapsed += dt`, `_tickTimer += dt`; при `_elapsed >= _duration` — `_pool.Release(gameObject)`, `_pool = null`, выйти; при `_tickRate > 0f && _tickTimer >= _tickRate` — `_tickTimer -= _tickRate` и для врагов в XZ-радиусе `_radius` вызвать `enemy.TryApplyHit(_tickDamage, enemy.transform.position, _onHit, _depth + 1)`. Реализовать `OnGetFromPool`/`OnReleaseFromPool` (сброс `_pool`/таймеров).
3) В `OnHitDispatcher`: поле `public GroundOnHitHandler ground;` и в `ProcessHit(..., int depth)` добавить `case OnHitModuleData.OnHitKind.Ground: if (ground != null && depth < module.maxDepth) ground.HandleHit(module, position, target, depth); break;`.

## Ограничения
- Зона — pooled (без Instantiate/Destroy). Урон/ре-триггер через `TryApplyHit` (T075). XZ-дистанция, как в `ExplosionOnHitHandler`.
- Без `new`/LINQ/`GetComponent` в `Update`. `IHitHandler` не менять (перегрузка `HandleHit(..., int depth)` — только у `GroundOnHitHandler`).
- namespace `TankSurvival.Combat`; `using TankSurvival.Core` (PoolManager, IPoolable), `using TankSurvival.Enemy` (EnemyController), `using TankSurvival.Data` (OnHitModuleData), `using UnityEngine`.

## Unity (человеку)
1. Создать префаб зоны: ПКМ в Hierarchy → 3D Object → Plane (или Cylinder) → назвать `GroundZoneVFX`; добавить компонент `GroundZone`; полупрозрачный материал; перетащить в `Assets/Prefabs/VFX/` (создаст `GroundZoneVFX.prefab`), объект из Hierarchy удалить.
2. `Assets/Data/OnHit/Ground.asset` → `groundVfx` → `GroundZoneVFX.prefab`.
3. В сцене создать пустой GameObject `GroundOnHitHandler` → добавить компонент `GroundOnHitHandler`; поле `pool` → объект `PoolManager`; поле `onHit` → компонент `OnHitDispatcher` (на `Turret`).

## Проверка человеком
- Проект компилируется без ошибок.
- `GroundOnHitHandler.pool` = PoolManager; `GroundOnHitHandler.onHit` = OnHitDispatcher.

## Отчёт Исполнителя
Готово:
Блокер:
