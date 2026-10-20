# T066: OnHitModuleData — новые kind и поля
- Status: [ ]
- Phase: 10
- Files:
  - Assets/Scripts/Data/OnHitModuleData.cs

## Задача
В `OnHitModuleData` расширить `enum OnHitKind` значениями `Slow`, `Freeze`, `Knockback`, `Ground` (после `Burn`). Добавить публичные поля: `float slowAmount`, `int freezeThreshold`, `float force`, `float damageScaling`, `GameObject groundVfx`, `int maxDepth = 5`. Существующие поля (`duration`, `tickRate`, `tickDamage`, `radius`, `priority`, `damage`, `explosionVfx`) не удалять и не переименовывать — их переиспользуют новые модули.

## Ограничения
- Не менять существующие поля и enum-значения `Explosion`/`Burn`.
- Только данные, без логики. namespace `TankSurvival.Data`; `using UnityEngine` уже есть.
- `maxDepth` — `public int maxDepth = 5` (field initializer, дефолт в инспекторе).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- `Create → Tank Survival → OnHit Module Data`: в инспекторе видны enum `Slow/Freeze/Knockback/Ground` и поля `slowAmount`, `freezeThreshold`, `force`, `damageScaling`, `groundVfx`, `maxDepth`.

## Отчёт Исполнителя
Готово:
Блокер:
