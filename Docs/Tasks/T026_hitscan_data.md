# T026: DeliveryModuleData — hitscan-поле maxTargets
- Status: [x]
- Phase: 4
- Files:
  - Assets/Scripts/Data/DeliveryModuleData.cs

## Задача
Добавить в `DeliveryModuleData` публичное поле `int maxTargets` — сколько целей поражает hitscan-луч на линии (для Beam = 1). Источник — `Docs/GDD/Turrets/Modules/Delivery/Hitscan.md`, поле `max_targets`. Значение задаёт человек в `.asset` (инспектор).

## Ограничения
- Существующие поля (`kind`, `speed`, `pelletCount`, `spreadAngle`, `range`, `aps`, `damage`) не трогать.
- enum `DeliveryKind { Projectile, Hitscan }` не менять (значение `Hitscan` уже есть).
- namespace `TankSurvival.Data` и `[CreateAssetMenu(...)]` не менять.

## Unity (человеку)
Создать ассеты (ПКМ в папке → Create → Tank Survival → нужный тип):
1. `Assets/Data/Delivery/Hitscan.asset` (DeliveryModuleData): `kind = Hitscan`, `maxTargets = 1`, `range = 18`, `aps = 4`, `damage = 90`; `speed = 0`, `pelletCount = 1`, `spreadAngle = 0` (для hitscan не используются).
2. `Assets/Data/Turrets/Beam.asset` (TurretData): `delivery` → `Hitscan.asset`.

Числа: `Hitscan.md` пресет `beam` + `Legend.md` (damage = база 10 × high 9 = 90; range = база 2.0 × high 9 = 18; aps = fast = 4). В `Beam.md` range указан как `extreme` (54) — расхождение с пресетом Hitscan; для MVP берём пресет Hitscan (18).

## Проверка человеком
- `Assets/Data/Delivery/Hitscan.asset` в инспекторе: `kind = Hitscan`, `maxTargets = 1`, `range = 18`, `aps = 4`, `damage = 90`.
- `Assets/Data/Turrets/Beam.asset` → `delivery` ссылается на `Hitscan.asset`.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: DeliveryModuleData.cs: добавлено публичное поле `int maxTargets`
Блокер: —