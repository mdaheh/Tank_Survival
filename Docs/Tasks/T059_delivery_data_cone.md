# T059: DeliveryModuleData — Cone + duration/tickRate/angle/coneVfx
- Status: [ ]
- Phase: 9
- Files:
  - Assets/Scripts/Data/DeliveryModuleData.cs

## Задача
В `DeliveryModuleData` (namespace `TankSurvival.Data`) расширить enum и добавить поля для Cone и continuous Hitscan:
1. В `enum DeliveryKind { Projectile, Hitscan }` добавить значение `Cone` (не менять порядок существующих значений).
2. Добавить `public enum DeliveryDuration { Instant, Continuous }` и поле `public DeliveryDuration duration;`.
3. Добавить поля `public float tickRate;`, `public float angle;`, `public GameObject coneVfx;`.

Источники: `Docs/GDD/Turrets/Modules/Delivery/Cone.md` (`angle`, `range`, `damage`, `duration: continuous`, `tick_rate`, `cone_vfx`) и `Docs/GDD/Turrets/Modules/Delivery/Hitscan.md` (`duration: instant|continuous`, `tick_rate`). Значения полей задаёт человек в `.asset` (инспектор) в T062; в коде значения не хардкодить.

## Ограничения
- Существующие поля (`kind`, `speed`, `pelletCount`, `spreadAngle`, `range`, `aps`, `damage`, `maxTargets`) не трогать.
- Порядок значений `DeliveryKind` не менять (`Projectile` остаётся первым, `Hitscan` вторым) — от него зависит routing в `TurretController` и существующие `.asset`.
- namespace `TankSurvival.Data` и `[CreateAssetMenu(...)]` не менять.
- Новые числовые поля — без дефолтных значений (числа кладёт человек); `duration` остаётся `Instant` по умолчанию (значение enum по умолчанию = 0).
- `using UnityEngine;` уже есть (нужен для `GameObject`); новых `using` не нужно.

## Unity (человеку)
Не требуется (поля появляются в инспекторе после компиляции; заполнение — в T062).

## Проверка человеком
- Проект компилируется без ошибок.
- Создать временный `DeliveryModuleData`-ассет → в инспекторе видны новые поля: `kind` (в т.ч. `Cone`), `duration` (`Instant`/`Continuous`), `tickRate`, `angle`, `coneVfx`.

## Отчёт Исполнителя
Готово:
Блокер:
