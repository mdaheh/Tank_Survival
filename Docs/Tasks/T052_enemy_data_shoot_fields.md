# T052: EnemyData — поля стрельбы (shootRange, shootInterval)
- Status: [x]
- Phase: 8
- Files:
  - Assets/Scripts/Data/EnemyData.cs

## Задача
В `EnemyData` (namespace `TankSurvival.Data`) добавить два публичных поля для
ranged-поведения: `public float shootRange = 8f;` и
`public float shootInterval = 1.5f;` — дистанция, с которой враг начинает
стрелять, и интервал между выстрелами. Melee-враги эти поля не используют.

## Ограничения
- Существующие поля (`health`, `speed`, `damage`, `xpReward`, `behavior`) не
  менять.
- Значения `8f` / `1.5f` — только дефолт инспектора; реальные значения
  задаются в `Shooter.asset` (см. T056).

## Unity (человеку)
- Не требуется (поля сериализуются автоматически).

## Проверка человеком
- Инспектор `Assets/Data/Enemies/Shooter.asset` (после T056) показывает поля
  `shootRange` и `shootInterval`.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: EnemyData.cs — добавлены поля `shootRange = 8f` и `shootInterval = 1.5f`.
Блокер: —
