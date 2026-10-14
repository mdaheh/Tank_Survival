# T040: Combat читает урон и скорострельность из StatBlock
- Status: [x]
- Phase: 6
- Depends: T039
- Files:
  - Assets/Scripts/Combat/TurretController.cs
  - Assets/Scripts/Combat/DeliveryHandler.cs
  - Assets/Scripts/Combat/ProjectileDeliveryHandler.cs
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs

## Задача
1. `DeliveryHandler`: изменить сигнатуру на `public abstract void Fire(Vector3 direction, float damage);`.
2. `ProjectileDeliveryHandler.Fire(Vector3 direction, float damage)`: в вызове `projectile.GetComponent<Projectile>().Init(...)` заменить первый аргумент `module.damage` на `damage` (остальные аргументы не менять).
3. `HitscanDeliveryHandler.Fire(Vector3 direction, float damage)`: заменить `_candidates[i].Damageable.TakeDamage(module.damage)` на `_candidates[i].Damageable.TakeDamage(damage)`.
4. `TurretController` (добавить `using TankSurvival.Core;`): добавить `public StatBlock statBlock;`; в `Update` заменить `active.Fire(direction)` на `active.Fire(direction, statBlock.damage)` и `_cooldown = 1f / turret.delivery.aps` на `_cooldown = 1f / statBlock.fireRate`.

## Ограничения
- Логику выбора handler'а (`turret.delivery.kind`), поиска цели и `turret.delivery.range` — не менять.
- `module.damage`/`turret.delivery.aps` больше НЕ используются как источник боевых чисел (только как базовое значение при `StatBlock.Init` в T039).

## Unity (человеку)
- Объект `Turret`: `TurretController.statBlock` → компонент `StatBlock` (на объекте `Player`).
- Объект `Turret_Beam`: `TurretController.statBlock` → компонент `StatBlock` (на объекте `Player`).

## Проверка человеком
- Инспектор `Turret` и `Turret_Beam`: `statBlock` заполнен.
- Play Mode: турель стреляет, урон равен `StatBlock.damage`, кулдаун = `1 / StatBlock.fireRate` (см. Smoke-тест фазы 6).
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/DeliveryHandler.cs — Fire(direction, damage); Assets/Scripts/Combat/ProjectileDeliveryHandler.cs — урон из аргумента damage; Assets/Scripts/Combat/HitscanDeliveryHandler.cs — урон из аргумента damage; Assets/Scripts/Combat/TurretController.cs — поле statBlock, Fire(direction, statBlock.damage), кулдаун 1f / statBlock.fireRate.
Блокер: —
