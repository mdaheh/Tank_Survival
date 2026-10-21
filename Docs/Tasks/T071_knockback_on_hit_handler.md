# T071: KnockbackOnHitHandler
- Status: [x]
- Phase: 10
- Depends: T066, T067
- Files:
  - Assets/Scripts/Combat/KnockbackOnHitHandler.cs

## Задача
Создать `KnockbackOnHitHandler : MonoBehaviour, IHitHandler` (namespace `TankSurvival.Combat`). В `HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)`: если `module == null || target == null || target.health == null || target.health.currentHealth <= 0f` — выйти. Вычислить XZ-направление от точки попадания: `Vector3 dir = target.transform.position - position; dir.y = 0f; dir.Normalize();`. Вызвать `target.ApplyKnockback(dir * (module.force * module.damageScaling), module.duration)`.

## Ограничения
- Сила = `force × damageScaling` (без проброса урона). Не толкать мёртвого. namespace `TankSurvival.Combat`; `using TankSurvival.Data`, `using TankSurvival.Enemy`, `using UnityEngine`.

## Unity (человеку)
1. В сцене создать пустой GameObject `KnockbackOnHitHandler` → добавить компонент `KnockbackOnHitHandler`.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/KnockbackOnHitHandler.cs: создан MonoBehaviour с интерфейсом IHitHandler; HandleHit с guard'ами по module/target/health считает XZ-направление от точки попадания и вызывает ApplyKnockback(dir * (module.force * module.damageScaling), module.duration).
Блокер: —
