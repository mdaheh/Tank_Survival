# T113: ShotData — SpawnRequest и Target
- Status: [ ]
- Phase: 12
- Depends: T112
- Files:
  - Assets/Scripts/Combat/Shooting/SpawnRequest.cs
  - Assets/Scripts/Combat/Shooting/Target.cs

## Задача
Создать два типа данных (namespace `TankSurvival.Combat.Shooting`, Shooting.md §11):
- `SpawnRequest`: поля `DeliveryType type; Vector3 position; Vector3 direction; BuildSnapshot buildSnapshot; int depth;` — запрос на спавн выстрела.
- `Target`: поля `Vector3 position; IDamageable damageable; int faction;` — цель попадания (позиция + получатель урона + фракция).

## Ограничения
- `Target` не ссылается на `EnemyController` (инвариант Shooting.md §14.12) — только `IDamageable` + данные.
- `SpawnRequest.buildSnapshot` — тип `BuildSnapshot` из этой же фазы (T115); forward-ссылка в одном проекте допустима.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
