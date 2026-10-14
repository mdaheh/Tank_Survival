# T035: Delivery — вызов ProcessHit при попадании
- Status: [x]
- Phase: 5
- Depends: T034
- Files:
  - Assets/Scripts/Combat/DeliveryHandler.cs
  - Assets/Scripts/Combat/Projectile.cs
  - Assets/Scripts/Combat/ProjectileDeliveryHandler.cs
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs

## Задача
Провести on_hit-слой через delivery. В `DeliveryHandler` (базовый) добавить `public OnHitDispatcher onHit;`. В `Projectile`: добавить `public OnHitDispatcher onHit;`, расширить `Init` до `Init(float damage, float speed, float range, Vector3 direction, PoolManager pool, OnHitDispatcher onHit)` с `this.onHit = onHit;`; в `Update` в ветке попадания после `enemy.Damageable.TakeDamage(damage)` вызвать `if (onHit != null) onHit.ProcessHit(enemy.transform.position, enemy);` перед `pool.Release(gameObject)`. В `ProjectileDeliveryHandler.Fire` передавать `onHit` в `Init`. В `HitscanDeliveryHandler.Fire` после `_candidates[i].Damageable.TakeDamage(module.damage)` вызвать `if (onHit != null) onHit.ProcessHit(_candidates[i].transform.position, _candidates[i]);`.

## Ограничения
- Механику полёта/попадания и вычисления XZ не менять — только добавить вызов `ProcessHit`.
- Без `new`, LINQ, `GetComponent` в `Update`/`Fire`-цикле.
- `OnHitDispatcher` — в `TankSurvival.Combat` (тот же namespace, `using` не нужен).

## Unity (человеку)
Не требуется (поля `onHit` заполняются из `TurretController` в T036).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: DeliveryHandler.cs: добавлено поле onHit, Projectile.cs: добавлено поле onHit, расширен Init и вызов ProcessHit в ветке попадания перед Release, ProjectileDeliveryHandler.cs: onHit передаётся в Init, HitscanDeliveryHandler.cs: вызов ProcessHit после TakeDamage
Блокер: —
