# T106: OnHitModuleData — kind и поля модификаторов
- Status: [-]
- Phase: 11
- Files:
  - Assets/Scripts/Data/OnHitModuleData.cs

## Задача
В `OnHitModuleData` расширить `enum OnHitKind` значениями `Pierce`, `Homing`, `BeamSplit`, `LaserFocus` (после `Ground`). Добавить публичные поля:
- `public int maxPierceCount = 3;` — GDD `Turrets/Modules/OnHit/Pierce.md` (`max_pierce_count`).
- `public float damageFalloff = 0.8f;` — GDD Pierce (`damage_falloff`, Legend §3.7: 0.8 = −20%).
- `public float homingStrength = 3f;` — GDD `Homing.md` (`homing_strength: medium` → Legend §1.8 medium = 3).
- `public float homingRadius = 3f;` — GDD Homing (`homing_radius: medium` → Legend §1.8 medium = 3).
- `public int splitCount = 3;` — GDD `Beam Split.md` (`split_count`).
- `public float splitAngle = 45f;` — GDD Beam Split (`split_angle: medium`; угол в градусах, как `DeliveryModuleData.angle`; базовый угол Legend §2.1 = 45°).
- `public float damageGrowth = 0.1f;` — GDD `Laser Focus.md` (`damage_growth`).
- `public int maxStacks = 5;` — GDD Laser Focus (`max_stacks`).
- `public float stackResetOnSwitch = 1f;` — GDD Laser Focus (`stack_reset_on_switch`, Legend §3.7: 1.0 = сброс всех).

## Ограничения
- Не менять существующие enum-значения и поля. Только данные, без логики.
- namespace `TankSurvival.Data`; `using UnityEngine` уже есть.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- `Create → Tank Survival → OnHit Module Data`: в инспекторе видны enum `Pierce/Homing/BeamSplit/LaserFocus` и новые поля с дефолтами выше.

## Отчёт Исполнителя
Готово:
Блокер:
