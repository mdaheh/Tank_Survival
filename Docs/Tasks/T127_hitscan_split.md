# T127: Разделение hitscan (instant + continuous)
- Status: [x]
- Phase: 14
- Depends: T126
- Files:
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/ShotFactory.cs
  - Assets/Scripts/Combat/Shooting/HitscanController.cs
  - Assets/Scripts/Combat/Shooting/InstantHitscanController.cs
  - Assets/Scripts/Combat/Shooting/ContinuousHitscanController.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs
  - Assets/Scripts/Combat/Shooting/Target.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Dev/DevVisualizer.cs

## Задача
Развести hitscan на два типа/контроллера (`Shooting.md` §14.4): мгновенный (Terminal, без Flight) и непрерывный (Periodic, самонаводящийся). Убрать общий `HitscanController`.

Справка (поля уже существуют, менять их не нужно): `ShotContext.currentTarget` имеет тип `Target` (`ShotContext.cs`); `Target` содержит `position` (Vector3), `damageable` (IDamageable), `enemy` (EnemyController), `faction` (int). `EnemyController` даёт статический список `Active`, поля `health`, `transform` и свойство `Damageable`.

1. `ShotTypes.cs`: в `DeliveryType` добавить значение `ContinuousHitscan` в конец перечисления (существующие значения не менять). `Hitscan` остаётся = мгновенный.

2. `ShotFactory.cs`: сменить сигнатуру на `private static DeliveryType MapType(DeliveryModuleData.DeliveryKind kind, DeliveryModuleData.DeliveryDuration duration)`; в `Build` вызывать `MapType(module.kind, module.duration)`. Для `DeliveryKind.Hitscan`: вернуть `ContinuousHitscan`, если `duration == DeliveryDuration.Continuous`, иначе `Hitscan`. `Cone` → `Cone`, default → `Projectile`. Поле `config.continuous` в `Build` не трогать; новые контроллеры его НЕ читают.

3. `InstantHitscanController` (новый файл, `namespace TankSurvival.Combat.Shooting`, stateless): перенести Terminal-логику старого `TickTerminal` из `HitscanController.cs`, но без Flight — весь цикл в одном тике:
   - `public void Tick(ShotContext ctx, float dt)`: `ctx.age += dt;`
   - `Vector3 origin = ctx.position; Vector3 end = origin + ctx.direction * ctx.config.range;`
   - debug-луч (в `#if UNITY_EDITOR || DEBUG`): `DevVisualizer.ShowBeam(origin, end, 0.15f, new Color(1f, 0.9f, 0.2f, 0.6f));`
   - цикл `for (int i = 0; i < ctx.config.maxTargets; i++)`: найти следующую цель вдоль луча (перенеси `FindNext`/`IsOnBeam` из старого `HitscanController.cs`, `skipHitTargets = true`); нет — `break`; построить `Target` (п.5), добавить в `ctx.hitTargets`, `ctx.hitPoint = target.position;`, вызвать все `ctx.buildSnapshot.hitModules[].OnHit(ctx, target)`.
   - после цикла: `ctx.keepAlive = false;` вызвать все `ctx.buildSnapshot.postHitModules[].OnPostHit(ctx);`
   - `if (!ctx.keepAlive) { ctx.destroyReason = DestroyReason.Hit; DestroyShot(ctx); }`

4. `ContinuousHitscanController` (новый файл, `namespace TankSurvival.Combat.Shooting`, stateless): Periodic Hit + самонаведение:
   - `public void Tick(ShotContext ctx, float dt)`: `ctx.age += dt;` если `ctx.state == ShotState.Spawn` → `ctx.state = ShotState.Flight;` если `ctx.state != ShotState.Flight` → `return`.
   - найти ближайшую валидную цель в радиусе `ctx.config.range`: перебрать `EnemyController.Active`, пропускать `enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f`; цель в радиусе, если `(enemy.transform.position - ctx.position).sqrMagnitude <= range * range`; взять с минимальной `sqrMagnitude`. Нет цели → `ctx.destroyReason = DestroyReason.External; DestroyShot(ctx); return;`
   - есть цель → если `ctx.currentTarget == null` — `ctx.currentTarget = new Target();`; заполнить `ctx.currentTarget.position = enemy.transform.position; ctx.currentTarget.damageable = enemy.Damageable; ctx.currentTarget.faction = 0; ctx.currentTarget.enemy = enemy;` затем `Vector3 toTarget = ctx.currentTarget.position - ctx.position; toTarget.y = 0f;` если `toTarget.sqrMagnitude > 1e-6f` → `ctx.direction = toTarget.normalized;`
   - debug-луч каждый кадр (в `#if UNITY_EDITOR || DEBUG`): `DevVisualizer.ShowBeam(ctx.position, ctx.position + ctx.direction * ctx.config.range, 0.15f, new Color(1f, 0.9f, 0.2f, 0.6f));`
   - урон: `if (ctx.age >= ctx.nextTickAt) { ctx.nextTickAt += ctx.config.tickRate; ... }` — внутри цикл `for (int i = 0; i < ctx.config.maxTargets; i++)` вдоль луча (`FindNext`/`IsOnBeam`, `skipHitTargets = false` — цели могут поражаться повторно каждый тик): построить `Target` (п.5), `ctx.hitPoint = target.position;`, вызвать `hitModules[].OnHit`; после цикла вызвать все `postHitModules[].OnPostHit(ctx)` один раз. `keepAlive` игнорируется.
   - `if (ctx.lifetime > 0f && ctx.age >= ctx.lifetime) { ctx.destroyReason = DestroyReason.Expired; DestroyShot(ctx); }`

5. Построение `Target` (общий приём в обоих контроллерах): `target.position = enemy.transform.position; target.damageable = enemy.Damageable; target.faction = 0; target.enemy = enemy;` Поле `enemy` заполнять всегда — его требует `LegacyOnHitModule` (T126).

6. `DestroyShot(ShotContext ctx)` (приватный, в обоих контроллерах): вызвать `ctx.buildSnapshot.destroyModules[].OnDestroy(ctx)`, затем `ctx.state = ShotState.Dead;`. Пул/финализацию делает `ShotSystem` (T129) — ссылку на него контроллеры не хранят.

7. Удалить `HitscanController.cs` (логика разошлась по двум новым файлам).

## Ограничения
- Не менять `ProjectileController`, `ConeController` (это T128).
- Контроллеры stateless: без ссылки на `ShotSystem` (Destroy = `ctx.state = Dead`; финализацию/пулы делает `ShotSystem`).
- Debug-маркер луча оставить на кадр полёта (`DevVisualizer.ShowBeam`).
- Новые контроллеры НЕ читают `ctx.config.continuous`.
- `using`: `TankSurvival.Enemy`, `UnityEngine`, и `#if UNITY_EDITOR || DEBUG using TankSurvival.Dev; #endif` (как в существующем `HitscanController.cs`).
- Без `new`/LINQ/`GetComponent`/`Find*` в циклах тика (политика проекта).

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T130) Laser бьёт непрерывно по ближайшей цели; при появлении более близкой перенацеливается; без цели — луч гаснет.

## Отчёт Исполнителя
Готово: ShotTypes.cs: в DeliveryType добавлено значение ContinuousHitscan (в конец); ShotFactory.cs: сигнатура MapType(DeliveryKind, DeliveryDuration) и вызов MapType(module.kind, module.duration), Hitscan+Continuous → ContinuousHitscan, иначе Hitscan; InstantHitscanController.cs: создан, Terminal-луч в одном тике (FindNext/IsOnBeam, hitTargets, hitPoint, OnHit, OnPostHit, DestroyShot); ContinuousHitscanController.cs: создан, Spawn→Flight, выбор ближайшей цели в радиусе range, поворот direction по цели, Periodic-тики по tickRate с postHit, Expired/External-уничтожение; HitscanController.cs: удалён (вместе с .meta)
Блокер: —
