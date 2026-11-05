# T136: ZoneController — Periodic AoE (наземная зона)
- Status: [ ]
- Phase: 15
- Depends: T134
- Files:
  - Assets/Scripts/Combat/Shooting/ZoneController.cs

## Задача
Создать `ZoneController` в `Assets/Scripts/Combat/Shooting/ZoneController.cs` (namespace `TankSurvival.Combat.Shooting`) — Periodic AoE для наземной зоны (`DeliveryType.Zone`), по образцу `ConeController`, но по кругу без угла и без самонаведения:

1. `public void Tick(ShotContext ctx, float dt)`:
   - `ctx.age += dt;`
   - если `ctx.state == ShotState.Spawn`: `ctx.state = ShotState.Flight; ctx.lifetime = ctx.config.duration;`
   - если `ctx.state != ShotState.Flight`: `return;`
   - если `ctx.lifetime > 0f && ctx.age >= ctx.lifetime`: `ctx.destroyReason = DestroyReason.Expired; DestroyShot(ctx); return;`
   - если `ctx.age >= ctx.nextTickAt`: `ctx.nextTickAt += ctx.config.tickRate;` и нанести тик: для каждого живого врага с `(transform.position - ctx.position).sqrMagnitude <= ctx.config.range * ctx.config.range` построить `Target`, установить `ctx.hitPoint = ctx.position;` и вызвать все `ctx.buildSnapshot.hitModules`.
2. `private static void DestroyShot(ShotContext ctx)`:
   - вызвать все `destroyModules.OnDestroy(ctx)`;
   - `ctx.state = ShotState.Dead;` (VFX смерти не спавнится).
3. Debug-маркер: `#if UNITY_EDITOR || DEBUG` вызывать `DevVisualizer.ShowCircle(ctx.position, ctx.config.range, Mathf.Max(0.1f, ctx.lifetime - ctx.age), new Color(0.4f, 0.9f, 0.4f, 0.35f));` — радиус зоны на оставшееся время жизни.

## Ограничения
- Stateless; позиция зоны неподвижна и задаётся модулем `GroundModule` (T138) через `SpawnRequest.position`.
- Без `angle` и без перенацеливания (наземная зона не наводится).
- `using`: `TankSurvival.Enemy`, `UnityEngine`; `TankSurvival.Dev` — под `#if UNITY_EDITOR || DEBUG`.

## Unity (человеку)
Не требуется (ground-визуал переносится в Phase 17; сейчас — debug-маркер).

## Проверка человеком
- (совместно с T140/T141) турель с `Ground`: зона наносит периодический урон в радиусе в течение `duration`, debug-круг виден и гаснет по истечении времени.

## Отчёт Исполнителя
Готово:
Блокер:
