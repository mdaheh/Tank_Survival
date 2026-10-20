# T060: ConeDeliveryHandler — конусный урон
- Status: [ ]
- Phase: 9
- Depends: T059
- Files:
  - Assets/Scripts/Combat/ConeDeliveryHandler.cs

## Задача
Создать `ConeDeliveryHandler : DeliveryHandler` (MonoBehaviour, namespace `TankSurvival.Combat`). Поля: `DeliveryModuleData module`, `PoolManager pool`, `Transform muzzle` (в `Awake`, если пусто — `transform`). В `Fire(Vector3 direction, float damage)`:
1. Если `module == null` — выйти.
2. Для каждого врага из `EnemyController.Active` проверить попадание в конус в XZ: `toEnemy = (враг.position - muzzle.position)` и `flatDir = direction`, оба с `y = 0`. Враг в конусе, если `toEnemy.sqrMagnitude <= module.range * module.range` и `Vector3.Angle(flatDir, toEnemy) <= module.angle * 0.5f`. Подходящим врагам вызвать `enemy.Damageable.TakeDamage(damage)` и `if (onHit != null) onHit.ProcessHit(enemy.transform.position, enemy);`.
3. Если `module.coneVfx != null && pool != null`: `GameObject vfx = pool.Get(module.coneVfx);` выставить `vfx.transform.position = muzzle.position`, развернуть по `direction` (вокруг `Vector3.up`), вызвать `vfx.GetComponent<PooledVFX>().Play(pool);`.

## Ограничения
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` (DeliveryModuleData), `using TankSurvival.Enemy` (EnemyController), `using TankSurvival.Core` (PoolManager), `using UnityEngine`.
- В `Fire` нет `new`, LINQ, `GetComponent` в цикле по врагам. `GetComponent<PooledVFX>` — один раз на вызов (не в цикле), допустимо.
- Угол и дальность считаются в XZ (Y игнорировать), как в `HitscanDeliveryHandler`.
- Существующие файлы (`DeliveryHandler.cs`, `ProjectileDeliveryHandler.cs`, `HitscanDeliveryHandler.cs`, `TurretController.cs`) не менять.
- Спавн VFX — через пул (`pool.Get`), без `Instantiate`.

## Unity (человеку)
Не требуется (компонент добавляется на турель в T062).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
