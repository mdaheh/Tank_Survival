# T062: EnemyKillSystem — отложенное освобождение мёртвых врагов
- Status: [x]
- Phase: 9.5
- Files:
  - Assets/Scripts/Enemy/EnemyKillSystem.cs

## Задача
Создать `EnemyKillSystem : MonoBehaviour` (namespace `TankSurvival.Enemy`). Поля: `public PoolManager pool;` и `private readonly List<GameObject> _pending = new List<GameObject>();`. Метод `public void Enqueue(GameObject enemy)` — если `enemy == null`, выйти; иначе `_pending.Add(enemy)`. В `private void LateUpdate()` — если `pool == null` или `_pending.Count == 0`, выйти; иначе для каждого `GameObject` из `_pending` вызвать `pool.Release(...)`, затем `_pending.Clear()`. Это единственная точка возврата мёртвых врагов в пул.

## Ограничения
- namespace `TankSurvival.Enemy`; `using System.Collections.Generic` (List), `using TankSurvival.Core` (PoolManager), `using UnityEngine`.
- Нет `new`/LINQ/`GetComponent`/`Find*`; `_pending` переиспользуется (Clear, не пересоздаётся).
- `Enqueue` не вызывает `pool.Release` сразу — только кладёт в очередь. Флаш только в `LateUpdate` (после всех `Update` игровой логики).
- Освобождение — только через `pool.Release`; логику пула не дублировать.
- Дубликаты в `_pending` не обрабатывать: `EnemyHealth.TakeDamage` выходит при `currentHealth <= 0`, поэтому `OnDeath`/`Enqueue` на одну смерть вызывается один раз.

## Unity (человеку)
- Hierarchy → создать пустой GameObject → назвать `EnemyKillSystem` → Add Component → `EnemyKillSystem`.
- `EnemyKillSystem` → компонент `EnemyKillSystem` → поле `Pool` → объект `PoolManager`.

## Проверка человеком
- Проект компилируется без ошибок.
- Инспектор `EnemyKillSystem`: поле `Pool` ссылается на `PoolManager`.

## Отчёт Исполнителя
Готово: Assets/Scripts/Enemy/EnemyKillSystem.cs — создан EnemyKillSystem (Enqueue в _pending, флаш pool.Release по всем элементам в LateUpdate, затем _pending.Clear()).
Блокер: —
