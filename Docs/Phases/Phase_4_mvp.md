# Phase 4: Beam (hitscan instant)
- Status: in-progress
- Цель: второй delivery-пайплайн — hitscan (мгновенный луч) — параллельно projectile. Beam-турель наносит урон через тот же `IDamageable` без спавна объектов; настройки Projectile не ломаются.

## Задачи (порядок = приоритет)
- T026: `DeliveryModuleData` — hitscan-поле `int maxTargets`.
- T027: `HitscanDeliveryHandler` — мгновенный вычислительный луч, урон через `IDamageable`.
- T028: `TurretController` — маршрутизация по `turret.delivery.kind` (projectile / hitscan).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] В сцене две турели: Hammer (Projectile) и Beam (Hitscan) — обе стреляют по ближайшему врагу.
- [ ] Beam: в момент выстрела враг мгновенно получает урон (нет летящего объекта), HP снимается через `EnemyHealth.TakeDamage`.
- [ ] Beam с `maxTargets = 1` поражает ровно одну цель на линии; враг вне `range` урона не получает.
- [ ] Projectile работает как раньше: снаряд из пула, попадает, HP снимается — `Projectile.asset` не сломан.
- [ ] Hitscan не создаёт/не уничтожает объекты (нет роста иерархии между выстрелами).
- [ ] В консоли нет ошибок.
