# T134: Площадной конфиг доставки (DeliveryConfig + DeliveryType.Zone)
- Status: [ ]
- Phase: 15
- Files:
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs

## Задача
Подготовить конфиг и тип доставки для площадных контроллеров (взрыв/зона):

1. В `DeliveryConfig` (struct) добавить два поля:
   - `public float duration;` — время жизни площадного выстрела (0 = не задано); источник — `OnHitModuleData.duration` у наземной зоны, заполняется в `GroundModule` (T138).
   - `public GameObject deathVfx;` — VFX при уничтожении (вспышка взрыва); источник — `OnHitModuleData.explosionVfx`, заполняется в `ExplosionModule` (T138).
2. В `ShotTypes.cs` в `enum DeliveryType` добавить значение `Zone` в конец списка (после `ContinuousHitscan`).

## Ограничения
- Существующие поля `DeliveryConfig` и порядок уже имеющихся значений `DeliveryType` не менять.
- `duration`/`deathVfx` заполняются только area-модулями (T138); обычные доставки оставляют их значениями по умолчанию.
- `DeliveryConfig` остаётся `struct` с `using UnityEngine;`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- В `ShotTypes.cs` присутствует `DeliveryType.Zone`; в `DeliveryConfig.cs` появились поля `duration` и `deathVfx`.

## Отчёт Исполнителя
Готово:
Блокер:
