# T108: Pierce — пробитие снарядом
- Status: [ ]
- Phase: 11
- Depends: T107
- Files:
  - Assets/Scripts/Combat/Projectile.cs

## Задача
Снаряд с модулем Pierce пробивает врагов насквозь: счётчик начинается с `maxPierceCount`, после каждого попадания уменьшается; пока счётчик > 0 снаряд летит дальше, урон следующей цели = `damage × damageFalloff`; при счётчике 0 снаряд возвращается в пул. Реализовать в `Projectile`:
1. Добавить `using TankSurvival.Data;` (для `OnHitModuleData`).
2. Новые приватные поля: `private int _pierceRemaining;`, `private float _damageFalloff = 1f;`, `private readonly List<EnemyController> _hitEnemies = new List<EnemyController>();` (`using System.Collections.Generic` уже есть).
3. В `Init(...)` после `this.onHit = onHit;`: `OnHitModuleData pierce = onHit != null ? onHit.GetModule(OnHitModuleData.OnHitKind.Pierce) : null; _pierceRemaining = pierce != null ? pierce.maxPierceCount : 0; _damageFalloff = pierce != null ? pierce.damageFalloff : 1f; _hitEnemies.Clear();`
4. В `Update` в блоке попадания (`if (distanceSqr <= HitRadiusSqr)`):
   - `if (_hitEnemies.Contains(enemy)) continue;`
   - `_hitEnemies.Add(enemy); enemy.TryApplyHit(damage, transform.position, onHit);`
   - `_pierceRemaining--;`
   - `if (_pierceRemaining > 0) { damage *= _damageFalloff; } else { pool.Release(gameObject); }`
   - `return;` (снаряд не возвращается в пул, пока есть пробития).
5. В `OnGetFromPool` и `OnReleaseFromPool`: добавить `_pierceRemaining = 0; _hitEnemies.Clear();`.

## Ограничения
- Без `new`/LINQ в `Update`; `List<EnemyController>` — поле-поле (переиспользуется). Сигнатуру `Init` не менять.
- namespace `TankSurvival.Combat`.

## Unity (человеку)
1. `Assets/Data/OnHit/` → создать `Pierce.asset` (Create → Tank Survival → OnHit Module Data): `kind = Pierce`, `maxPierceCount = 3`, `damageFalloff = 0.8`, `priority = 2`.
2. `Assets/Data/Turrets/Hummer.asset` (Projectile delivery) → `onHitModules` добавить `Pierce.asset`.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Dev Menu → выбрать турель Hummer → Start Round. Снаряд попадает в первого врага, НЕ исчезает, летит дальше; урон следующего врага ×0.8; после 3 пробитий снаряд исчезает. (Пространственного маркера нет — механика-счётчик.)

## Отчёт Исполнителя
Готово:
Блокер:
