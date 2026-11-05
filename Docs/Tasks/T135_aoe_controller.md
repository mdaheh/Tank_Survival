# T135: AoEController — Terminal AoE (взрыв)
- Status: [ ]
- Phase: 15
- Depends: T134
- Files:
  - Assets/Scripts/Combat/Shooting/AoEController.cs

## Задача
Создать `AoEController` в `Assets/Scripts/Combat/Shooting/AoEController.cs` (namespace `TankSurvival.Combat.Shooting`) — Terminal AoE для взрыва (`DeliveryType.AoE`), по образцу `InstantHitscanController` (без стадии Flight, вся цепочка за первый тик):

1. `public void Tick(ShotContext ctx, float dt)`:
   - `ctx.age += dt;`
   - `ctx.hitPoint = ctx.position;` (центр взрыва — точка, откуда распространяется эффект);
   - `float rangeSqr = ctx.config.range * ctx.config.range;` пройти `EnemyController.Active`, пропуская `null` и `health == null` или `currentHealth <= 0f`; для врага с `(transform.position - ctx.position).sqrMagnitude <= rangeSqr`: построить `Target` (как `BuildTarget` в `InstantHitscanController`), добавить в `ctx.hitTargets`, вызвать все `ctx.buildSnapshot.hitModules` на эту цель;
   - после обхода: `ctx.keepAlive = false;` вызвать все `postHitModules.OnPostHit(ctx)`; `keepAlive` для AoE игнорируется — всегда `ctx.destroyReason = DestroyReason.Hit; DestroyShot(ctx);`.
2. `private static void DestroyShot(ShotContext ctx)`:
   - вызвать все `destroyModules.OnDestroy(ctx)`;
   - если `ctx.config.deathVfx != null` — спавнить через `IVfxSpawner.Spawn(ctx.config.deathVfx, ctx.position)` (ленивый кэш `IVfxSpawner _vfx` + свойство `Vfx`, как в `ConeController`);
   - `ctx.state = ShotState.Dead;`.
3. Debug-маркер: `#if UNITY_EDITOR || DEBUG` вызывать `DevVisualizer.ShowCircle(ctx.position, ctx.config.range, 0.3f, new Color(1f, 0.25f, 0.2f, 0.5f));`.

## Ограничения
- Stateless; без аллокаций в `Tick` (кроме целевых `Target`, которые строятся явно).
- НЕ совмещать Periodic-режим: это отдельный `ZoneController` (T136).
- `using`: `TankSurvival.Core` (`GameServices`), `TankSurvival.Enemy` (`EnemyController`), `UnityEngine`; `TankSurvival.Dev` — под `#if UNITY_EDITOR || DEBUG`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T140/T141) турель с `Explosion`: при попадании все враги в радиусе получают урон, виден debug-круг (`DevVisualizer.ShowCircle`) и вспышка VFX (`deathVfx`).

## Отчёт Исполнителя
Готово:
Блокер:
