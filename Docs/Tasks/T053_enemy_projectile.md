# T053: EnemyProjectile — вражеский снаряд (пул)
- Status: [x]
- Phase: 8
- Files:
  - Assets/Scripts/Enemy/EnemyProjectile.cs (новый)

## Задача
Создать класс `EnemyProjectile : MonoBehaviour, IPoolable` (namespace
`TankSurvival.Enemy`; `using TankSurvival.Core;`, `using UnityEngine;`).

Поля:
- `public float damage;`
- `public float speed = 10f;` — скорость снаряда, задаётся на префабе.
- `public float range;` — дистанция пробега до возврата в пул.
- `public Vector3 direction;`
- `public PoolManager pool;`
- `private IDamageable _targetDamageable;`
- `private Transform _target;`
- `private float _travelled;`
- `private const float HitRadiusSqr = 1f;`

Методы:
- `public void Init(float damage, float range, Vector3 direction,
  PoolManager pool, Transform target)`: сохранить аргументы в поля;
  `_targetDamageable = target.GetComponent<IDamageable>(); _target = target;
  _travelled = 0f;`.
- `private void Update()`: если `pool == null` — `return`. Иначе
  `float step = speed * Time.deltaTime; transform.Translate(direction * step,
  Space.World); _travelled += step;`. Если `_travelled >= range` →
  `pool.Release(gameObject); return;`. Если `_target == null` →
  `pool.Release(gameObject); return;`. Иначе если
  `(transform.position - _target.position).sqrMagnitude <= HitRadiusSqr` →
  `_targetDamageable.TakeDamage(damage); pool.Release(gameObject);`.
- `OnGetFromPool()` / `OnReleaseFromPool()`: `_travelled = 0f; direction =
  Vector3.zero;`.

## Ограничения
- Без `GetComponent` в `Update`: `_targetDamageable`/`_target` кэшируются в
  `Init`.
- Без `Instantiate`/`Destroy` — снаряд живёт только в пуле (`IPoolable`).
- `speed` — сериализуемое поле префаба, не константа в логике.

## Unity (человеку)
1. Project → `Assets/Prefabs/Enemies` → создать пустой GameObject
   `EnemyProjectile`, добавить компонент `EnemyProjectile` (`speed` = 10).
   Сохранить как префаб `Assets/Prefabs/Enemies/EnemyProjectile.prefab`
   (визуал — на усмотрение человека, обязателен только компонент).

## Проверка человеком
- `EnemyProjectile.prefab` в `Assets/Prefabs/Enemies` с компонентом
  `EnemyProjectile` и `speed` = 10.
- Проект компилируется без ошибок.
- Поведение снаряда проверяется в T054/T056 (Shooter стреляет).

## Отчёт Исполнителя
Готово: EnemyProjectile.cs — новый класс MonoBehaviour, IPoolable (пул, Init, Update по ТЗ, сброс в OnGet/OnRelease).
Блокер: —
