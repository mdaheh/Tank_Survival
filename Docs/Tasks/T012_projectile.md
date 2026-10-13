# T012: Снаряд — Projectile
- Status: [x]
- Phase: 2
- Priority: High
- Depends: T010, T011
- Files:
  - Assets/Scripts/Combat/Projectile.cs

## Задача
`Projectile` (MonoBehaviour, `IPoolable`, namespace `TankSurvival.Combat`): поля
`float damage`, `float speed`, `float range`, `Vector3 direction`, `PoolManager pool`.
Метод `void Init(float damage, float speed, Vector3 direction, PoolManager pool)`.
В `Update`: `transform.Translate(direction * speed * Time.deltaTime, Space.World)` с накоплением
пройденного пути; при `путь ≥ range` → `pool.Release(gameObject)`. Каждый кадр проход по
`EnemyController.Active`: если `(enemy.transform.position - transform.position).sqrMagnitude ≤ 0.25`
(hitRadius 0.5) → `enemy.health.TakeDamage(damage)` и `pool.Release(gameObject)`.
`OnGetFromPool`/`OnReleaseFromPool` сбрасывают пройденный путь/направление.

## Ограничения
- Без физики: попадание — по дистанции до активных врагов, не через коллайдеры.
- В `Update` нет `new`, LINQ, `GetComponent` в цикле; урон через `enemy.health` (кэш).
- namespace `TankSurvival.Combat`; `using TankSurvival.Core` (PoolManager), `using TankSurvival.Enemy` (EnemyController).

## Unity (человеку)
Создать префаб снаряда:
1. ПКМ в Hierarchy → 3D Object → Sphere → назвать `Projectile`.
2. Добавить компонент `Projectile`.
3. Перетащить объект в `Assets/Prefabs/Projectiles/` (создаст `Projectile.prefab`); объект из Hierarchy удалить.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Projectile.cs: MonoBehaviour + IPoolable в TankSurvival.Combat — поля damage/speed/range/direction/pool, Init, Update с Translate по direction в Space.World и накоплением пути (Release при путь ≥ range), попадание по sqrMagnitude ≤ 0.25 по списку EnemyController.Active через кэш enemy.health без GetComponent в цикле, OnGetFromPool/OnReleaseFromPool сбрасывают путь и направление
Блокер: —
