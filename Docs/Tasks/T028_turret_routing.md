# T028: TurretController — маршрутизация по delivery.kind
- Status: [x]
- Phase: 4
- Depends: T026, T027
- Files:
  - Assets/Scripts/Combat/TurretController.cs

## Задача
В `TurretController` добавить поле `DeliveryHandler hitscanDelivery` (существующее `delivery` остаётся projectile-handler'ом). В `Update` вычислять активный handler по `turret.delivery.kind`: `Hitscan` → `hitscanDelivery`, иначе → `delivery`. Существующую раннюю проверку `delivery == null` заменить на проверку выбранного handler'а (`== null` → `return`). При выстреле вызывать `active.Fire(direction)`. Кулдаун (`1f / turret.delivery.aps`) и поиск ближайшего врага не менять.

## Ограничения
- `turret.delivery.range` / `turret.delivery.aps` для поиска цели и кулдауна — не менять.
- В `Update` нет `new`, LINQ, `GetComponent` в цикле, `Find*`.
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` уже есть (для `DeliveryModuleData.DeliveryKind`).
- Сигнатуру `DeliveryHandler.Fire(Vector3 direction)` не менять.

## Unity (человеку)
1. На существующем объекте `Turret` (Hammer): `TurretController` → поле `hitscanDelivery` оставить пустым.
2. В Hierarchy продублировать объект `Turret` → переименовать копию в `Turret_Beam` (оставить дочерним объектом игрока).
3. На `Turret_Beam` удалить компонент `ProjectileDeliveryHandler`; добавить компонент `HitscanDeliveryHandler`.
4. `Turret_Beam` → `TurretController`: `turret` → `Beam.asset`, `delivery` → пусто, `hitscanDelivery` → компонент `HitscanDeliveryHandler`.
5. `Turret_Beam` → `HitscanDeliveryHandler`: `module` → `Hitscan.asset`, `muzzle` → `Turret_Beam`.

## Проверка человеком
- В Play Mode: Hammer стреляет снарядом, Beam мгновенно снимает HP — обе по ближайшему врагу.
- Инспектор `Turret_Beam` → `TurretController`: `turret = Beam.asset`, `hitscanDelivery = HitscanDeliveryHandler`.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: TurretController.cs: добавлено поле `hitscanDelivery`, в `Update` выбор handler'а по `turret.delivery.kind` (Hitscan → hitscanDelivery, иначе → delivery), ранняя проверка перенесена на выбранный handler, выстрел через `active.Fire(direction)`
Блокер: —