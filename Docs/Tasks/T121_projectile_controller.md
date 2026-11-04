# T121: ProjectileController — снаряд (Terminal Hit, swept)
- Status: [ ]
- Phase: 13
- Depends: T119
- Files:
  - Assets/Scripts/Combat/Shooting/ProjectileController.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/Target.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Dev/DevVisualizer.cs

## Задача
Создать `ProjectileController` (class, stateless, namespace `TankSurvival.Combat.Shooting`) —
контроллер `DeliveryType.Projectile` (Shooting.md §12, §13). Публичный метод:
`public void Tick(ShotContext ctx, float dt)`.

Логика тика (Spawn-инициализация временно в Tick, до появления ShotSystem в Phase 14):
1. `ctx.age += dt`.
2. Если `ctx.state == ShotState.Spawn` → `ctx.state = Flight; ctx.remainingRange = ctx.config.range;`.
3. Если `ctx.state != Flight` → `return`.
4. `foreach (m in ctx.buildSnapshot.flightModules) m.OnFlight(ctx, dt)`.
5. `velocity = ctx.direction * ctx.buildSnapshot.speed`; `prevPosition = position`;
   `position += velocity * dt`; `remainingRange -= velocity.magnitude * dt`.
6. Expired (`remainingRange <= 0 || age >= lifetime`) → `destroyReason = DestroyReason.Expired` → Destroy.
7. Swept-поиск целей по `EnemyController.Active`: пропуск мёртвых
   (`enemy.health == null || enemy.health.currentHealth <= 0f`) и уже вошедших в `ctx.hitTargets`;
   попадание если расстояние точка→сегмент(`prevPosition`,`position`) ≤ 1f (`HitRadiusSqr`, как в legacy `Projectile`).
8. На каждую новую цель: добавить в `ctx.hitTargets`; собрать `Target { position = enemy.transform.position,
   damageable = enemy.Damageable, faction = 0 }`; `ctx.hitPoint = точка попадания`;
   `foreach (m in ctx.buildSnapshot.hitModules) m.OnHit(ctx, target)`.
9. `ctx.keepAlive = false`; `foreach (m in ctx.buildSnapshot.postHitModules) m.OnPostHit(ctx)`.
10. Если `!ctx.keepAlive` → `destroyReason = DestroyReason.Hit` → Destroy.

Destroy: `foreach (m in ctx.buildSnapshot.destroyModules) m.OnDestroy(ctx); ctx.state = ShotState.Dead;`.

## Ограничения
- usings (полный блок):
  ```
  using TankSurvival.Enemy;
  using UnityEngine;
  #if UNITY_EDITOR || DEBUG
  using TankSurvival.Dev;
  #endif
  ```
  Тип `IDamageable` напрямую не называется (доступ через `enemy.Damageable` и
  `Target.damageable`), поэтому `using TankSurvival.Core;` не требуется.
- Stateless; контроллер не двигает Transform (визуал — Phase 14).
- `keepAlive` сохранить — нужно пробитию (Pierce, Phase 16).
- Dev-маркер (#if UNITY_EDITOR || DEBUG) на кадр полёта:
  `DevVisualizer.ShowBeam(ctx.prevPosition, ctx.position, Time.deltaTime, new Color(1f, 0.5f, 0f, 0.5f))`.

## Unity (человеку)
Не требуется (класс не подключается в сцену).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
