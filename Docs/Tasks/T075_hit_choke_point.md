# T075: Hit choke point — TryApplyHit + ProcessHit(depth)
- Status: [ ]
- Phase: 10
- Files:
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Combat/OnHitDispatcher.cs
  - Assets/Scripts/Combat/Projectile.cs
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs
  - Assets/Scripts/Combat/ConeDeliveryHandler.cs

## Задача
1) В `EnemyController` добавить `using TankSurvival.Combat;` и метод `public bool TryApplyHit(float damage, Vector3 hitPos, OnHitDispatcher onHit, int depth = 0)`: если `health == null || health.currentHealth <= 0f` — вернуть `false` (труп не бьём и не триггерим on-hit); иначе `health.TakeDamage(damage)` (может убить), затем `onHit?.ProcessHit(hitPos, this, depth)` и вернуть `true`.
2) В `OnHitDispatcher` вынести тело `ProcessHit` в перегрузку с глубиной: `public void ProcessHit(Vector3 position, EnemyController target)` → вызывает `ProcessHit(position, target, 0)`; `public void ProcessHit(Vector3 position, EnemyController target, int depth)` содержит тот же цикл (case `Explosion`/`Burn` без изменений) и в начале `if (target == null) return;`. Параметр `depth` пока не используется (задействует Ground в T072).
3) Заменить пары «`TakeDamage` + `ProcessHit`» на `TryApplyHit`:
   - `Projectile.Update`: `enemy.TryApplyHit(damage, enemy.transform.position, onHit); pool.Release(gameObject); return;`
   - `HitscanDeliveryHandler.Fire`: `_candidates[i].TryApplyHit(damage, _candidates[i].transform.position, onHit);`
   - `ConeDeliveryHandler.Fire`: `enemy.TryApplyHit(damage, enemy.transform.position, onHit);`

## Ограничения
- `TryApplyHit` — ЕДИНСТВЕННОЕ место guard'а «живой?». НЕ ставить `currentHealth <= 0` внутрь цикла `ProcessHit` — это сломает on-hit по убивающему выстрелу.
- Порядок урон → on-hit сохранить. Логику поиска цели/геометрии в delivery не менять.
- `using TankSurvival.Combat` — только в `EnemyController.cs`; в остальных файлах `using` не раздувать.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Hammer/Beam/Flamethrower наносят урон как раньше; Explosion/Burn при наличии on-hit срабатывают.

## Отчёт Исполнителя
Готово:
Блокер:
