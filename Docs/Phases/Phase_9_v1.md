# Phase 9: Delivery полностью — Cone + continuous Hitscan
- Status: in-progress
- Цель: закрыть delivery-пайплайн — третий модуль Cone (конусный урон) и режим `continuous` у Hitscan (непрерывный луч с тиками). Projectile и Hitscan `instant` (Beam/Railgun) не ломаются.

## Задачи (порядок = приоритет)
- T059: `DeliveryModuleData` — `DeliveryKind.Cone`, enum `DeliveryDuration` и поля `duration`/`tickRate`/`angle`/`coneVfx`.
- T060: `ConeDeliveryHandler` — конусный свип (угол/дальность) + `onHit` + `coneVfx` из пула.
- T061: `TurretController` — маршрутизация `Cone` и каденция по `duration` (instant → aps, continuous → tickRate).

## Setup for smoke test (human)

Перед smoke-тестом собрать две турели данными (код не меняется — используются
`DeliveryModuleData`/`ConeDeliveryHandler`/`TurretController` из T059–T061).

Числа (GDD + Legend; база урона 10, как в Hammer/Beam; база range 2.0):
- Flamethrower (Cone): `kind` = Cone, `duration` = Continuous, `angle` = 60, `range` = 6, `damage` = 10, `tickRate` = 4, `coneVfx` = `ConeVFX.prefab`.
- Laser (Hitscan continuous): `kind` = Hitscan, `duration` = Continuous, `tickRate` = 4, `maxTargets` = 1, `range` = 18, `damage` = 30.
- `angle`: Legend даёт формулу 45°×3=135° (для угла некорректна — high/extreme дают >360°); для MVP берём полный угол конуса 60° (±30° от направления).

Шаги:
1. Project → `Assets/Data/Delivery` → Create → Tank Survival → Delivery Module Data:
   - `Cone.asset`: `kind`=Cone, `duration`=Continuous, `angle`=60, `range`=6, `damage`=10, `tickRate`=4, `coneVfx` → `ConeVFX.prefab` (шаг 3). `speed`/`pelletCount`/`spreadAngle`/`aps`/`maxTargets` = 0/1 (для Cone не используются).
   - `Laser.asset`: `kind`=Hitscan, `duration`=Continuous, `tickRate`=4, `maxTargets`=1, `range`=18, `damage`=30. `aps`/`speed`/`pelletCount`/`spreadAngle`/`angle`/`coneVfx` = 0/null.
2. Project → `Assets/Data/Turrets` → Create → Tank Survival → Turret Data:
   - `Flamethrower.asset`: `delivery` → `Cone.asset`; `onHitModules` — пусто.
   - `Laser.asset`: `delivery` → `Laser.asset` (delivery-ассет из шага 1); `onHitModules` — пусто.
3. Project → `Assets/Prefabs/VFX` → создать префаб `ConeVFX` (конус/цилиндр-меш, полупрозрачный материал) с компонентом `PooledVFX`, `duration` ≈ 0.25 (интервал тика).
4. Hierarchy → продублировать объект `Turret` (Hammer) дважды → переименовать в `Turret_Flamethrower` и `Turret_Laser` (оставить дочерними игрока):
   - `Turret_Flamethrower`: удалить `ProjectileDeliveryHandler`; добавить `ConeDeliveryHandler`. `TurretController`: `turret` → `Flamethrower.asset`, `delivery`/`hitscanDelivery` → пусто, `coneDelivery` → компонент `ConeDeliveryHandler`, `statBlock` → `StatBlock` (на `Player`), `onHit` → как на `Turret`. `ConeDeliveryHandler`: `module` → `Cone.asset`, `muzzle` → `Turret_Flamethrower`, `pool` → объект `PoolManager`.
   - `Turret_Laser`: удалить `ProjectileDeliveryHandler`; добавить `HitscanDeliveryHandler`. `TurretController`: `turret` → `Laser.asset`, `hitscanDelivery` → компонент `HitscanDeliveryHandler`, `delivery`/`coneDelivery` → пусто, `statBlock` → `StatBlock`, `onHit` → как на `Turret`. `HitscanDeliveryHandler`: `module` → `Laser.asset`, `muzzle` → `Turret_Laser`.
5. Для smoke-теста: `SessionState.selectedTurret` → соответствующий `.asset` (чтобы `StatBlock.damage` совпал с per-tick уроном). Включать/отключать нужный объект `Turret_*` в Hierarchy.

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] Flamethrower (Cone): все враги в конусе перед турелью получают урон тиками (`tickRate` fast); враги вне угла/`range` урона не получают; `coneVfx` проигрывается.
- [x] Laser (Hitscan continuous): луч наносит урон ближайшему врагу тиками (`tickRate` fast); при появлении более близкого врага переключается на него; при смерти цели — на новую (или стоп); вне `range` не бьёт.
- [x] Beam/Railgun (Hitscan instant) не сломаны: мгновенный выстрел по `aps`, `maxTargets` соблюдается.
- [x] Hammer (Projectile) не сломан: снаряд из пула, урон при контакте.
- [x] Число объектов в иерархии не растёт (Cone VFX возвращается в пул; непрерывный луч не спавнит объекты).
- [x] Консоль чистая.
