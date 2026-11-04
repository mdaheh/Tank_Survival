# T129: ShotSystem — оркестратор lifecycle
- Status: [ ]
- Phase: 14
- Depends: T124, T125, T127
- Files:
  - Assets/Scripts/Combat/Shooting/ShotSystem.cs

## Задача
Создать `ShotSystem : MonoBehaviour` в `TankSurvival.Combat.Shooting` — рантайм-оркестратор выстрелов.

Поля (сериализованные):
- `ShotView shotViewPrefab;` — визуал снаряда (тип Projectile).
- `GameObject coneVfxPrefab;` — VFX конуса (тип Cone).
- `int maxDepth = 10;` — лимит вложенности детей.

Приватное состояние:
- пул контекстов (`Stack<ShotContext>`); активные выстрелы (каждый: монотонный `id`, `ctx`, `view` (может быть `null`), контроллер); счётчик `_nextId`;
- по одному экземпляру `ProjectileController`, `InstantHitscanController`, `ContinuousHitscanController`, `ConeController`;
- `PoolManager`/`IVfxSpawner` — через `GameServices` (кэшировать).

`public int Fire(SpawnRequest request)`:
1. Взять `ctx` из пула (иначе `new ShotContext()`), `ctx.Reset()`.
2. Скопировать `position`/`direction`/`buildSnapshot`/`config`/`depth`; `ctx.state = ShotState.Spawn`.
3. Вызвать `buildSnapshot.spawnModules → OnSpawn(ctx)`; обработать `spawnQueue`.
4. По `request.type`:
   - `Projectile`: `pool.Get(shotViewPrefab.gameObject)` → `GetComponent<ShotView>().Bind(ctx)` (сохранить view);
   - `Cone`: `ivfxSpawner?.Spawn(coneVfxPrefab, ctx.position, ctx.direction)`;
   - `Hitscan`/`ContinuousHitscan`/`AoE`: визуал не создаётся (debug-маркер в контроллере; `AoE` — Phase 15).
5. Записать активный выстрел и вернуть монотонный `id` (`_nextId++`).

`Update()`: работать по копии активных; для каждого `controller.Tick(ctx, dt)`; затем `FlushSpawnQueue(ctx)`; если `ctx.state == ShotState.Dead` — вернуть `view` в пул (`pool.Release`), вернуть `ctx` в пул, убрать выстрел из активных.

`public bool IsAlive(int id)`: true, если `id` есть среди активных.

`FlushSpawnQueue(ctx)`: извлекать `SpawnRequest child`; если `child.depth > maxDepth` — пропустить; иначе `Fire(child)`.

## Ограничения
- Контекст не хранит ссылку на GameObject; ноль `Instantiate` в `Update`.
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
