# T054: EnemyController — ranged-поведение (стрельба)
- Status: [ ]
- Phase: 8
- Depends: T052, T053
- Files:
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Data/EnemyData.cs
  - Assets/Scripts/Enemy/EnemyProjectile.cs

## Задача
Расширить `EnemyController` (namespace `TankSurvival.Enemy`) ranged-веткой.

1. Добавить поля: `public GameObject projectilePrefab;` (ссылка на префаб
   вражеского снаряда), `private PoolManager _pool;`,
   `private float _shootCooldown;`.
2. В `Init(...)`: сохранить пул — `_pool = pool;` (перед `health.Init`).
3. В `Update()`: в начале, рядом с `_attackCooldown -= Time.deltaTime;`,
   добавить `_shootCooldown -= Time.deltaTime;`. После расчёта `distanceSqr`
   и до melee-блока вставить: если `data.behavior == "ranged"` и
   `distanceSqr > AttackRange * AttackRange`:
   - если `distanceSqr > data.shootRange * data.shootRange` → двигаться к
     игроку (текущий `MoveTowards`) и `return`;
   - иначе (в зоне стрельбы): если `_shootCooldown <= 0f &&
     projectilePrefab != null && _pool != null` →
     `Vector3 dir = (target.position - transform.position).normalized;`
     `GameObject p = _pool.Get(projectilePrefab); p.transform.position =
     transform.position; p.GetComponent<EnemyProjectile>().Init(data.damage,
     data.shootRange * 2f, dir, _pool, target); _shootCooldown =
     data.shootInterval;` затем `return`.
4. Melee-блок (текущий `if (_attackCooldown <= 0f ...)`) остаётся без
   изменений — он срабатывает и для ranged при `distanceSqr <= AttackRange^2`.

## Ограничения
- Логику melee (`AttackRange`, `AttackInterval`) не менять.
- Без `GetComponent` в `Update` — `p.GetComponent<EnemyProjectile>()` только
  в момент выстрела (раз в `shootInterval`).
- Дистанция пробега снаряда = `shootRange * 2f` (запас на долёт до цели,
  стоящей на границе `shootRange`).

## Unity (человеку)
1. На префабе `Assets/Prefabs/Enemies/Shooter.prefab` (создаётся в T056) в
   компоненте `EnemyController` заполнить `projectilePrefab` →
   `Assets/Prefabs/Enemies/EnemyProjectile.prefab`.
   (На melee-префабах Runner/Tank/Brute/Boss поле оставить пустым.)

## Проверка человеком
- Инспектор `Shooter.prefab` → `EnemyController` → `projectilePrefab`
  ссылается на `EnemyProjectile.prefab`.
- Play Mode (после T056): Shooter останавливается на дистанции `shootRange`,
  стреляет снарядами с интервалом `shootInterval`, снаряд попадает в игрока
  и наносит урон; при контакте Shooter бьёт melee.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
