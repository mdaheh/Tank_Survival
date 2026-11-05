# Phase 15: Стрельба v2 — миграция on-hit (статусы + area)
- Status: accepted
- Цель: заменить мост `LegacyOnHitModule` на настоящие lifecycle-модули on-hit (Burn/Slow/Freeze/Knockback/Explosion/Ground) и ввести площадные контроллеры `AoEController` (взрыв) и `ZoneController` (наземная зона), сохранив поведение текущей стрельбы.

## Задачи (порядок = приоритет)
- T134: Площадной конфиг доставки (`DeliveryConfig.duration/deathVfx`, `DeliveryType.Zone`).
- T135: `AoEController` — Terminal AoE (взрыв).
- T136: `ZoneController` — Periodic AoE (наземная зона).
- T137: Модули статусов/контроля (`BurnModule`, `SlowModule`, `FreezeModule`, `KnockbackModule`).
- T138: Модули area (`ExplosionModule`, `GroundModule`) — `IOnHit` + `spawnQueue`.
- T139: `ShotFactory` — сборка on-hit модулей из `OnHitModuleData[]`.
- T140: `ShotSystem` — подключить `AoEController`/`ZoneController`.
- T141: `TurretController` — передать `turret.onHitModules` в `ShotFactory`, убрать `OnHitDispatcher`.

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] Hammer: снаряд по-прежнему наносит базовый урон (без on-hit модулей).
- [x] Турель с `Explosion`: попадание создаёт взрыв — AoE-урон всем в радиусе (debug-круг), вспышка VFX.
- [x] Турель с `Burn`: враг получает горение — тики урона, burn снимает freeze.
- [x] Турель с `Ground`: зона наносит периодический урон в радиусе (`duration`, `tickRate`).
- [x] Турель с `Slow`/`Freeze`/`Knockback`: враг замедляется / останавливается / отталкивается.
- [x] Урон не дублируется: legacy `OnHitDispatcher` больше не вызывается.
- [x] Объекты не накапливаются: счётчик объектов стабилен.

## Примечания
- Миграция строго по `Docs/Shooting.md`: Explosion/Ground → `IOnHit` + `spawnQueue` (дочерний `ShotContext` area-типа); статусы → `IOnHit` (состояние остаётся владением `StatusEffectSystem`/`ControlEffectSystem`).
- `LegacyOnHitModule`, `OnHitDispatcher` и legacy-обработчики остаются в коде, но не используются — зачистка Phase 17.
- Ground-визуал (`groundVfx`) пока не переносится: префаб требует переделки с legacy `GroundZone`; проверка — debug-маркер (`DevVisualizer.ShowCircle`). Перенос визуала — Phase 17.
- AoE-дети наследуют билд избирательно: взрыв — `Without<ExplosionModule>`, зона — `Without<GroundModule>`; урон ребёнка задаётся из SO (взрыв `damage`, зона `tickDamage`).
