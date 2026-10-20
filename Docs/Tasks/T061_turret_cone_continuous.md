# T061: TurretController — маршрутизация Cone и каденция по duration
- Status: [ ]
- Phase: 9
- Depends: T059
- Files:
  - Assets/Scripts/Combat/TurretController.cs

## Задача
В `TurretController`:
1. Добавить `public DeliveryHandler coneDelivery;`.
2. В `Start()` добавить привязку on-hit: `if (coneDelivery != null) coneDelivery.onHit = onHit;` (после существующих `delivery`/`hitscanDelivery`).
3. В `Update()` заменить выбор активного handler'а на ветку по `turret.delivery.kind`: `Hitscan` → `hitscanDelivery`, `Cone` → `coneDelivery`, иначе → `delivery`.
4. В `Update()` добавить каденцию по `turret.delivery.duration`:
   - вычислить `float interval = turret.delivery.duration == DeliveryModuleData.DeliveryDuration.Continuous ? 1f / turret.delivery.tickRate : 1f / statBlock.fireRate;`
   - кулдаун по-прежнему через `_cooldown -= Time.deltaTime;` и `if (closest != null && _cooldown <= 0f)`.
   - при выстреле `_cooldown = interval;` (вместо текущего `1f / statBlock.fireRate`).
   - если `duration == Continuous` и `tickRate <= 0f` — не стрелять (защита от деления на ноль).

Поиск ближайшего врага (в пределах `turret.delivery.range`) и `active.Fire(direction, statBlock.damage)` не менять.

## Ограничения
- `turret.delivery.range` для поиска цели — не менять.
- В `Update` нет `new`, LINQ, `GetComponent` в цикле, `Find*`.
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` уже есть (для `DeliveryModuleData.DeliveryKind` и `DeliveryDuration`).
- Сигнатуру `DeliveryHandler.Fire(Vector3 direction, float damage)` не менять.
- `HitscanDeliveryHandler`/`ProjectileDeliveryHandler`/`ConeDeliveryHandler` в этой задаче не менять (continuous-луч — это каденция в `TurretController`; сам `Fire` уже делает один свип).

## Unity (человеку)
Не требуется (поле `coneDelivery` заполняется в T062).

## Проверка человеком
- Проект компилируется без ошибок.
- Инспектор существующей турели: `TurretController` показывает новое поле `coneDelivery` (пустое для Hammer/Beam).

## Отчёт Исполнителя
Готово:
Блокер:
