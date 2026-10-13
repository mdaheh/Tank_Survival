# T018: Выровнять модель стрельбы под GDD — параметры на delivery-модуле
- Status: [x]
- Phase: 2
- Priority: Medium
- Depends: T016
- Files:
  - Assets/Scripts/Data/DeliveryModuleData.cs
  - Assets/Scripts/Data/TurretData.cs
  - Assets/Scripts/Combat/TurretController.cs
  - Assets/Scripts/Combat/DeliveryHandler.cs
  - Assets/Scripts/Combat/ProjectileDeliveryHandler.cs
  - Assets/Scripts/Combat/Projectile.cs
  - Assets/Scripts/Enemy/EnemyController.cs

## Задача
Привести расположение параметров стрельбы к GDD: все параметры выстрела живут в `DeliveryModuleData`.
1. `DeliveryModuleData`: добавить `float aps`, `float damage` (к существующим kind/speed/range/pelletCount/spreadAngle).
2. `TurretData`: убрать `aps`, `range`, `damage`; оставить только `DeliveryModuleData delivery`.
3. `TurretController`: читать `turret.delivery.aps` (кулдаун), `turret.delivery.range` (наведение), `turret.delivery.damage` (урон); вызывать `delivery.Fire(direction)` без аргумента damage.
4. `DeliveryHandler`: сигнатура `void Fire(Vector3 direction);`.
5. `ProjectileDeliveryHandler.Fire(direction)`: использовать `module.damage`, `module.speed`, `module.range`, `module.pelletCount`, `module.spreadAngle`.
6. `Projectile`: урон наносить через `IDamageable` — заменить `enemy.health.TakeDamage(damage)` на `enemy.Damageable.TakeDamage(damage)`.
7. `EnemyController`: добавить свойство `public IDamageable Damageable => health;`.

## Ограничения
- namespace и логику пулов/движения не менять.
- `range` теперь один источник — `DeliveryModuleData.range` (и для наведения, и для полёта). Значение 6 (GDD medium); это отменяет рекомендацию «10» из T016.
- В `Update` без `new`, LINQ, `GetComponent` в цикле.
- Интеграцию `StatBlock` (damage/fire_rate из StatBlock) не делать — это фаза 6.

## Unity (человеку)
- `Assets/Data/Delivery/Projectile.asset`: выставить aps = 2, damage = 30, range = 6 (speed = 15, pelletCount = 1, spreadAngle = 0 — как было).
- `Assets/Data/Turrets/Hammer.asset`: оставить только `delivery` → `Projectile.asset` (поля aps/range/damage исчезнут из инспектора).

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Турень стреляет с aps 2, наводится на дальности 6, снаряд летит до 6 и наносит 30 урона (поведение как после T016, но дальность единая — 6).

## Отчёт Исполнителя
Готово: DeliveryModuleData.cs: добавлены aps и damage; TurretData.cs: осталось только поле delivery; TurretController.cs: наведение/кулдаун/урон читаются из turret.delivery (range/aps/damage), вызов delivery.Fire(direction), в guard добавлена проверка turret.delivery; DeliveryHandler.cs: сигнатура Fire(Vector3 direction); ProjectileDeliveryHandler.cs: Fire(direction) использует module.damage/speed/range/pelletCount/spreadAngle; Projectile.cs: урон через enemy.Damageable.TakeDamage(damage); EnemyController.cs: добавлено свойство public IDamageable Damageable => health
Блокер: —