# T062: Flamethrower + Laser — data-only (SO + префабы + сцена)
- Status: [ ]
- Phase: 9
- Depends: T059, T060, T061
- Files:
  - (нет — задача data-only, правок `.cs` нет)

## Задача
Добавить две турели данными: Flamethrower (Cone continuous) и Laser (Hitscan continuous) — без изменения кода. Создать delivery-ассеты, turret-ассеты, префаб `ConeVFX` и объекты сцены `Turret_Flamethrower`/`Turret_Laser`.

## Ограничения
- Код не меняется (`DeliveryModuleData`/`ConeDeliveryHandler`/`TurretController` готовы в T059–T061).
- Числа — из GDD + Legend (база урона 10, как в существующих Hammer/Beam; база range 2.0):
  - Flamethrower: `kind = Cone`, `duration = Continuous`, `angle = 60`, `range = 6`, `damage = 10`, `tickRate = 4`, `coneVfx = ConeVFX.prefab`.
  - Laser: `kind = Hitscan`, `duration = Continuous`, `tickRate = 4`, `maxTargets = 1`, `range = 18`, `damage = 30`.
- `angle` в Legend даёт формулу 45°×3=135° (для угла некорректна: high/extreme дают >360°). Для MVP берём полный угол конуса 60° (±30° от направления).

## Unity (человеку)
1. Project → `Assets/Data/Delivery` → Create → Tank Survival → Delivery Module Data:
   - `Cone.asset`: `kind` = Cone, `duration` = Continuous, `angle` = 60, `range` = 6, `damage` = 10, `tickRate` = 4, `coneVfx` → `ConeVFX.prefab` (создать в шаге 3). Поля `speed`/`pelletCount`/`spreadAngle`/`aps`/`maxTargets` — 0/1 (для Cone не используются).
   - `Laser.asset`: `kind` = Hitscan, `duration` = Continuous, `tickRate` = 4, `maxTargets` = 1, `range` = 18, `damage` = 30. Поля `aps`/`speed`/`pelletCount`/`spreadAngle`/`angle`/`coneVfx` — не используются (0/null).
2. Project → `Assets/Data/Turrets` → Create → Tank Survival → Turret Data:
   - `Flamethrower.asset`: `delivery` → `Cone.asset`; `onHitModules` — пусто.
   - `Laser.asset`: `delivery` → `Laser.asset` (delivery-ассет из шага 1); `onHitModules` — пусто.
3. Project → `Assets/Prefabs/VFX` → создать префаб `ConeVFX` (конус/цилиндр-меш, полупрозрачный материал) с компонентом `PooledVFX`, `duration` ≈ 0.25 (интервал тика).
4. Hierarchy → продублировать объект `Turret` (Hammer) дважды → переименовать в `Turret_Flamethrower` и `Turret_Laser` (оставить дочерними игрока):
   - `Turret_Flamethrower`: удалить `ProjectileDeliveryHandler`; добавить `ConeDeliveryHandler`. `TurretController`: `turret` → `Flamethrower.asset`, `delivery` → пусто, `hitscanDelivery` → пусто, `coneDelivery` → компонент `ConeDeliveryHandler`, `statBlock` → `StatBlock` (на `Player`), `onHit` → как на `Turret`.
   - `Turret_Laser`: удалить `ProjectileDeliveryHandler`; добавить `HitscanDeliveryHandler`. `TurretController`: `turret` → `Laser.asset`, `hitscanDelivery` → компонент `HitscanDeliveryHandler`, `delivery`/`coneDelivery` → пусто, `statBlock` → `StatBlock`, `onHit` → как на `Turret`.
   - `ConeDeliveryHandler` на `Turret_Flamethrower`: `module` → `Cone.asset`, `muzzle` → `Turret_Flamethrower`, `pool` → объект `PoolManager`.
   - `HitscanDeliveryHandler` на `Turret_Laser`: `module` → `Laser.asset`, `muzzle` → `Turret_Laser`.
5. Для smoke-теста: `SessionState.selectedTurret` → соответствующий `.asset` (чтобы `StatBlock.damage` совпал с per-tick уроном). Включать/отключать нужный объект `Turret_*` в Hierarchy.

## Проверка человеком
- `Cone.asset`: kind=Cone, duration=Continuous, angle=60, range=6, damage=10, tickRate=4, coneVfx заполнен.
- `Laser.asset`: kind=Hitscan, duration=Continuous, tickRate=4, maxTargets=1, range=18, damage=30.
- `Flamethrower.asset`/`Laser.asset` (turret) ссылаются на свои delivery.
- Play Mode (см. smoke-тест фазы): Flamethrower бьёт конусом по тикам, Laser — непрерывным лучом.

## Отчёт Исполнителя
Готово:
Блокер:
