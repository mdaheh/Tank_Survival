# T027: HitscanDeliveryHandler — мгновенный луч
- Status: [ ]
- Phase: 4
- Depends: T026
- Files:
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs

## Задача
Создать `HitscanDeliveryHandler : DeliveryHandler` (MonoBehaviour, namespace `TankSurvival.Combat`). Поля: `DeliveryModuleData module`, `Transform muzzle` (в `Awake`, если пусто — `transform`). В `Fire(Vector3 direction)`: если `module == null` — выйти; построить отрезок `muzzle.position → muzzle.position + direction * module.range` (XZ, Y игнорировать); найти до `module.maxTargets` врагов из `EnemyController.Active`, чей центр в XZ на расстоянии ≤ 1.0 от отрезка, отсортированных по удалению от `muzzle` вдоль луча; каждому вызвать `enemy.Damageable.TakeDamage(module.damage)`. Без `Physics.Raycast`.

## Ограничения
- Без физики: попадание — вычислительный луч (расстояние от центра врага до отрезка в XZ), как свип в `Projectile`.
- В `Fire` нет `new`, LINQ, `GetComponent` в цикле.
- Приватный статический `DistanceToSegmentXZSqr(Vector3 point, Vector3 a, Vector3 b)` — продублировать формулу из `Projectile` (без аллокаций).
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` (DeliveryModuleData), `using TankSurvival.Enemy` (EnemyController), `using System.Collections.Generic`, `using UnityEngine`.
- `Projectile.cs`, `ProjectileDeliveryHandler.cs`, `DeliveryHandler.cs` не менять.
- Для Phase 4 `maxTargets = 1` — поражается один ближайший враг.

## Unity (человеку)
Не требуется (компонент добавляется на турель в T028).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер: