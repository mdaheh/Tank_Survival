# T112: ShotTypes — enum состояния, причины и типы доставки
- Status: [ ]
- Phase: 12
- Files:
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs

## Задача
Создать файл `Assets/Scripts/Combat/Shooting/ShotTypes.cs` (namespace
`TankSurvival.Combat.Shooting`) с тремя enum:
- `ShotState`: `Spawn`, `Flight`, `Hit`, `PostHit`, `Destroy`, `Dead` (Shooting.md §4, §12).
- `DestroyReason`: `Hit`, `Expired`, `External` (Shooting.md §6.5).
- `DeliveryType`: `Projectile`, `Hitscan`, `Cone`, `AoE` (Shooting.md §13, Architecture.md §7).

## Ограничения
- Только enum-ы, без полей и логики.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
