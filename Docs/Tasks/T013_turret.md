# T013: Турель — TurretController + DeliveryHandler + ProjectileDeliveryHandler
- Status: [ ]
- Phase: 2
- Priority: Critical
- Depends: T009, T010, T012
- Files:
  - Assets/Scripts/Combat/TurretController.cs
  - Assets/Scripts/Combat/DeliveryHandler.cs
  - Assets/Scripts/Combat/ProjectileDeliveryHandler.cs

## Задача
`DeliveryHandler` (абстрактный MonoBehaviour, namespace `TankSurvival.Combat`):
`public abstract void Fire(Vector3 direction, float damage);`.
`ProjectileDeliveryHandler` (наследник, namespace `TankSurvival.Combat`): поля
`DeliveryModuleData module`, `GameObject projectilePrefab`, `PoolManager pool`,
`Transform muzzle` (если пусто — `transform`). `Fire(direction, damage)`: `module.pelletCount`
раз взять `pool.Get(projectilePrefab)`, выставить позицию в `muzzle`, вызвать
`projectile.GetComponent<Projectile>().Init(damage, module.speed, direction + spread, pool)`,
где spread — равномерный разброс в пределах `module.spreadAngle`.
`TurretController` (MonoBehaviour, namespace `TankSurvival.Combat`): поля `TurretData turret`,
`DeliveryHandler delivery`; таймер кулдауна. В `Update`: найти ближайшего врага из
`EnemyController.Active` в пределах `turret.range` (ручной цикл по `sqrMagnitude`, без LINQ);
если цель есть и кулдаун ≤ 0 → `delivery.Fire((цель - transform.position).normalized, turret.damage)`
и сбросить кулдаун на `1 / turret.aps`.

## Ограничения
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` (TurretData, DeliveryModuleData), `using TankSurvival.Enemy` (EnemyController), `using TankSurvival.Core` (PoolManager).
- В `Update` нет `new`, LINQ, `GetComponent` в цикле, `Find*`.
- Направление считается от `TurretController.transform`; спавн — от `muzzle` у `ProjectileDeliveryHandler`.

## Unity (человеку)
На объекте игрока:
1. Создать пустой дочерний GameObject `Turret` (расположить над танком).
2. Добавить на `Turret` компоненты `TurretController` и `ProjectileDeliveryHandler`.
3. `TurretController`: `turret` → `Hammer.asset`, `delivery` → компонент `ProjectileDeliveryHandler`.
4. `ProjectileDeliveryHandler`: `module` → `Projectile.asset`, `projectilePrefab` → `Projectile.prefab`, `muzzle` → `Turret` (сам объект).
   Поле `pool` заполнить в T014.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
