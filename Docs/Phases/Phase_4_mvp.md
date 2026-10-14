# Phase 4: Beam (hitscan instant)
- Status: accepted
- Цель: второй delivery-пайплайн — hitscan (мгновенный луч) — параллельно projectile. Beam-турель наносит урон через тот же `IDamageable` без спавна объектов; настройки Projectile не ломаются.

## Задачи (порядок = приоритет)
- T026: `DeliveryModuleData` — hitscan-поле `int maxTargets`.
- T027: `HitscanDeliveryHandler` — мгновенный вычислительный луч, урон через `IDamageable`.
- T028: `TurretController` — маршрутизация по `turret.delivery.kind` (projectile / hitscan).

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] В сцене две турели: Hammer (Projectile) и Beam (Hitscan) — обе стреляют по ближайшему врагу.
- [x] Beam: в момент выстрела враг мгновенно получает урон (нет летящего объекта), HP снимается через `EnemyHealth.TakeDamage`.
- [x] Beam с `maxTargets = 1` поражает ровно одну цель на линии; враг вне `range` урона не получает.
- [x] Projectile работает как раньше: снаряд из пула, попадает, HP снимается — `Projectile.asset` не сломан.
- [x] Hitscan не создаёт/не уничтожает объекты (нет роста иерархии между выстрелами).
- [x] В консоли нет ошибок.
