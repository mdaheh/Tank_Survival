# T033: ExplosionOnHitHandler + PooledVFX — AoE + VFX из пула
- Status: [x]
- Phase: 5
- Depends: T030
- Files:
  - Assets/Scripts/Combat/ExplosionOnHitHandler.cs
  - Assets/Scripts/Combat/PooledVFX.cs

## Задача
1) `PooledVFX : MonoBehaviour, IPoolable` (namespace `TankSurvival.Combat`): поле `float duration = 1f`; приватные `PoolManager _pool`, `float _timer`. `public void Play(PoolManager pool)` — `_pool = pool`, `_timer = duration`. В `Update`: если `_pool == null` — выйти; `_timer -= Time.deltaTime`; при `_timer <= 0` — `_pool.Release(gameObject)` и `_pool = null`. `OnGetFromPool`/`OnReleaseFromPool` сбрасывают `_timer`/`_pool`.
2) `ExplosionOnHitHandler : MonoBehaviour, IHitHandler` (namespace `TankSurvival.Combat`): поле `public PoolManager pool`. В `HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)`: если `module` null — выйти; AoE — пройти `EnemyController.Active`, для каждого врага в XZ в радиусе `module.radius` (`dx² + dz² ≤ radius²`) вызвать `enemy.Damageable.TakeDamage(module.damage)`; затем, если `module.explosionVfx != null && pool != null` — `pool.Get(module.explosionVfx)`, выставить позицию в `position`, `GetComponent<PooledVFX>().Play(pool)`.

## Ограничения
- В `HandleHit`/`Update` нет `new`, LINQ, `GetComponent` в цикле; XZ-дистанция, как в `Projectile`/`HitscanDeliveryHandler`.
- `target` в `ExplosionOnHitHandler` не используется (AoE — по `position` + `EnemyController.Active`).
- namespace `TankSurvival.Combat`; `using TankSurvival.Core` (PoolManager, IPoolable), `using TankSurvival.Enemy` (EnemyController), `using TankSurvival.Data` (OnHitModuleData), `using UnityEngine`, `using System.Collections.Generic`.

## Unity (человеку)
1. Создать префаб VFX: ПКМ в Hierarchy → 3D Object → Sphere → назвать `ExplosionVFX`; добавить компонент `PooledVFX` (`duration = 0.5`); по желанию дочерний `ParticleSystem` с `Play On Awake`. Перетащить в `Assets/Prefabs/VFX/` (создаст `ExplosionVFX.prefab`), объект из Hierarchy удалить.
2. `Assets/Data/OnHit/Explosion.asset` → `explosionVfx` → `ExplosionVFX.prefab`.
3. В сцене создать пустой GameObject `ExplosionOnHitHandler` → добавить компонент `ExplosionOnHitHandler`; поле `pool` → объект `PoolManager`.

## Проверка человеком
- `Explosion.asset` → `explosionVfx` ссылается на `ExplosionVFX.prefab`.
- `ExplosionOnHitHandler.pool` = PoolManager.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: PooledVFX.cs: создан MonoBehaviour, IPoolable с Play/таймером в Update и возвратом в PoolManager, ExplosionOnHitHandler.cs: создан IHitHandler с AoE-уроном по EnemyController.Active в радиусе и спавном VFX из пула
Блокер: —
