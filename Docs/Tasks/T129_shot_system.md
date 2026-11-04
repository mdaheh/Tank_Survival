# T129: ShotSystem — оркестратор lifecycle
- Status: [ ]
- Phase: 14
- Depends: T124, T125, T127
- Files:
  - Assets/Scripts/Combat/Shooting/ShotSystem.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs
  - Assets/Scripts/Combat/Shooting/SpawnRequest.cs
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/ShotView.cs
  - Assets/Scripts/Combat/Shooting/IVfxSpawner.cs
  - Assets/Scripts/Combat/Shooting/ProjectileController.cs
  - Assets/Scripts/Combat/Shooting/InstantHitscanController.cs
  - Assets/Scripts/Combat/Shooting/ContinuousHitscanController.cs
  - Assets/Scripts/Combat/Shooting/ConeController.cs
  - Assets/Scripts/Core/PoolManager.cs
  - Assets/Scripts/Core/GameServices.cs

## Задача
Создать `ShotSystem : MonoBehaviour` в `TankSurvival.Combat.Shooting` — рантайм-оркестратор выстрелов. Перед T129 завершите T127 (он даёт `InstantHitscanController` и `ContinuousHitscanController` — без них `ShotSystem` не скомпилируется).

Поля (сериализованные):
- `ShotView shotViewPrefab;` — визуал снаряда (тип Projectile).
- `GameObject coneVfxPrefab;` — VFX конуса (тип Cone).
- `int maxDepth = 10;` — лимит вложенности детей.

Приватное состояние:
- `Stack<ShotContext> _pool;`
- `List<ActiveShot> _active;` — вложенный приватный `class ActiveShot { public int id; public ShotContext ctx; public ShotView view; public DeliveryType type; }` (`view` может быть `null`; `type` нужен для выбора контроллера в `Update`); счётчик `int _nextId;`
- по одному экземпляру `ProjectileController _projectile;`, `InstantHitscanController _instantHitscan;`, `ContinuousHitscanController _continuousHitscan;`, `ConeController _cone;` (создать один раз);
- `PoolManager _poolManager;` и `IVfxSpawner _vfx;` — лениво через `GameServices.Get<PoolManager>()` / `GameServices.Get<IVfxSpawner>()` при первом обращении (кэшировать в поле).

`public int Fire(SpawnRequest request)`:
1. Взять `ctx` из пула (иначе `new ShotContext()`), `ctx.Reset()`.
2. Скопировать `position`/`direction`/`buildSnapshot`/`config`/`depth`; `ctx.state = ShotState.Spawn`.
3. Вызвать `buildSnapshot.spawnModules → OnSpawn(ctx)`; обработать `spawnQueue`.
4. По `request.type`:
   - `Projectile`: `GameObject go = _poolManager.Get(shotViewPrefab.gameObject); ShotView view = go.GetComponent<ShotView>(); view.Bind(ctx);` (сохранить `view`);
   - `Cone`: `_vfx?.Spawn(coneVfxPrefab, ctx.position, ctx.direction);` (`view` = null);
   - `Hitscan`/`ContinuousHitscan`: визуал не создаётся (`view` = null); debug-маркер делает контроллер;
   - `AoE`: не порождается в Phase 14 (`ShotFactory` не мапит `AoE`) — ветку можно не реализовывать.
5. `int id = _nextId++;` добавить `new ActiveShot { id = id, ctx = ctx, view = view, type = request.type }` в `_active`; вернуть `id`.

`Update()`:
- `float dt = Time.deltaTime;`
- `for (int i = _active.Count - 1; i >= 0; i--)` (обратный порядок, чтобы безопасно удалять без копирования списка): взять `ActiveShot s = _active[i];`
  - выбрать контроллер по `s.type`: `Projectile` → `_projectile`, `Hitscan` → `_instantHitscan`, `ContinuousHitscan` → `_continuousHitscan`, `Cone` → `_cone`; вызвать `controller.Tick(s.ctx, dt);`
  - `FlushSpawnQueue(s.ctx);`
  - если `s.ctx.state == ShotState.Dead`: если `s.view != null` → `_poolManager.Release(s.view.gameObject);` затем `_pool.Push(s.ctx);` и `_active.RemoveAt(i);`

`public bool IsAlive(int id)`: true, если `id` есть среди активных.

`FlushSpawnQueue(ShotContext ctx)`: `while (ctx.spawnQueue.Count > 0)` — взять последний `SpawnRequest child` (`ctx.spawnQueue[Count - 1]` + `RemoveAt(Count - 1)`); если `child.depth > maxDepth` — пропустить; иначе `Fire(child)`.

## Ограничения
- Контекст не хранит ссылку на GameObject; ноль `Instantiate` в `Update` (создание `GameObject` — только внутри `PoolManager.Get`).
- В `Update` не аллоцировать список: итерировать `_active` индексами в обратном порядке (без `ToArray`/LINQ/копии).
- `using`: `TankSurvival.Core`, `UnityEngine`, `System.Collections.Generic`.
- VFX — только через `IVfxSpawner`; VFX не создаёт `ShotContext`.
- `Fire`/`IsAlive` — без событий: инициатор опрашивает `IsAlive(id)`.

## Unity (человеку)
1. В сцене создать GameObject `ShotSystem` → Add Component → `ShotSystem`.
2. Поле `Shot View Prefab` → из Project `Assets/Prefabs/Projectiles/Projectile.prefab`.
3. Поле `Cone Vfx Prefab` → из Project `Assets/Prefabs/VFX/ConeVFX.prefab`.

## Проверка человеком
- Inspector → объект `ShotSystem` → поля `Shot View Prefab` и `Cone Vfx Prefab` заполнены.
- (совместно с T130) выстрелы появляются и исчезают, число активных объектов не растёт.

## Отчёт Исполнителя
Готово:
Блокер:
