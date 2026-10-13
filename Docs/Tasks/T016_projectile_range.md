# T016: Дальность полёта снаряда — передать range из данных
- Status: [x]
- Phase: 2
- Priority: Critical
- Depends: T009, T012, T013
- Files:
  - Assets/Scripts/Data/DeliveryModuleData.cs
  - Assets/Scripts/Combat/Projectile.cs
  - Assets/Scripts/Combat/ProjectileDeliveryHandler.cs

## Задача
`DeliveryModuleData`: добавить публичное поле `float range`.
`Projectile.Init`: изменить сигнатуру на `Init(float damage, float speed, float range, Vector3 direction, PoolManager pool)` и добавить `this.range = range;`.
`ProjectileDeliveryHandler.Fire`: передавать `module.range` — вызов `projectile.GetComponent<Projectile>().Init(damage, module.speed, module.range, spreadDirection, pool);`.

## Ограничения
- Поле `range` в `Projectile` и проверку `_distanceTravelled >= range` не менять.
- Значение `range` задаёт человек в `Projectile.asset` (рекомендуем 10, не меньше дальности турели 6).
- namespace и прочую структуру классов не менять.

## Unity (человеку)
В `Assets/Data/Delivery/Projectile.asset` (DeliveryModuleData) выставить `range = 10`.

## Проверка человеком
- Снаряд летит и при попадании снимает 30 HP; враг (20 HP) гибнет с одного выстрела и возвращается в пул.

## Отчёт Исполнителя
Готово: DeliveryModuleData.cs: добавлено публичное поле float range; Projectile.cs: сигнатура Init расширена до Init(float damage, float speed, float range, Vector3 direction, PoolManager pool) с присвоением this.range (поле range и проверка _distanceTravelled >= range не менялись); ProjectileDeliveryHandler.cs: в Fire передаётся module.range в вызов Init
Блокер: —