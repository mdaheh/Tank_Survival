# Phase 5: On-hit модули
- Status: in-progress
- Цель: on_hit-слой поверх delivery — `IHitHandler` + `OnHitDispatcher` (список по priority) + `StatusEffectSystem` (тики DoT). Explosion наносит AoE-урон + VFX из пула; Burn накладывает DoT с тиками каждые `tick_rate` в течение `duration`; модули берутся из `TurretData.onHitModules`, один выстрел может нести несколько эффектов.

## Задачи (порядок = приоритет)
- T029: `OnHitModuleData` (kind + поля) и `TurretData.onHitModules`.
- T030: `IHitHandler` — контракт on_hit-модуля.
- T031: `StatusEffectSystem` — тики burn-DoT.
- T032: `BurnOnHitHandler` — наложение DoT.
- T033: `ExplosionOnHitHandler` + `PooledVFX` — AoE + VFX из пула.
- T034: `OnHitDispatcher` — маршрутизация по priority.
- T035: Delivery — вызов `ProcessHit` при попадании.
- T036: `TurretController` — привязка on_hit-слоя.

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] Hammer (Projectile) + Explosion: при попадании снаряда все враги в радиусе получают урон; проигрывается VFX взрыва в точке попадания.
- [x] Несколько выстрелов Explosion подряд: число активных объектов в иерархии не растёт (VFX из пула и возвращается, нет Instantiate/Destroy).
- [x] Beam (Hitscan) + Burn: попавший враг получает DoT — HP снимается порциями каждые 0.5 с в течение 3 с, затем урон прекращается.
- [x] Враг, убитый DoT, возвращается в пул; burn-статус снят (нет тиков по мёртвому/спящему объекту).
- [x] Projectile/Hitscan с пустым `onHitModules` работают как раньше — не сломаны.
- [x] В консоли нет ошибок.
