# Phase 9: Delivery полностью — Cone + continuous Hitscan
- Status: in-progress
- Цель: закрыть delivery-пайплайн — третий модуль Cone (конусный урон) и режим `continuous` у Hitscan (непрерывный луч с тиками). Projectile и Hitscan `instant` (Beam/Railgun) не ломаются.

## Задачи (порядок = приоритет)
- T059: `DeliveryModuleData` — `DeliveryKind.Cone`, enum `DeliveryDuration` и поля `duration`/`tickRate`/`angle`/`coneVfx`.
- T060: `ConeDeliveryHandler` — конусный свип (угол/дальность) + `onHit` + `coneVfx` из пула.
- T061: `TurretController` — маршрутизация `Cone` и каденция по `duration` (instant → aps, continuous → tickRate).
- T062: Flamethrower + Laser — data-only (SO + префабы + объекты сцены).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Flamethrower (Cone): все враги в конусе перед турелью получают урон тиками (`tickRate` fast); враги вне угла/`range` урона не получают; `coneVfx` проигрывается.
- [ ] Laser (Hitscan continuous): луч наносит урон ближайшему врагу тиками (`tickRate` fast); при появлении более близкого врага переключается на него; при смерти цели — на новую (или стоп); вне `range` не бьёт.
- [ ] Beam/Railgun (Hitscan instant) не сломаны: мгновенный выстрел по `aps`, `maxTargets` соблюдается.
- [ ] Hammer (Projectile) не сломан: снаряд из пула, урон при контакте.
- [ ] Число объектов в иерархии не растёт (Cone VFX возвращается в пул; непрерывный луч не спавнит объекты).
- [ ] Консоль чистая.
