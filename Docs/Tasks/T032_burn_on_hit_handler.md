# T032: BurnOnHitHandler — наложение DoT
- Status: [ ]
- Phase: 5
- Depends: T030, T031
- Files:
  - Assets/Scripts/Combat/BurnOnHitHandler.cs

## Задача
Создать `BurnOnHitHandler : MonoBehaviour, IHitHandler` (namespace `TankSurvival.Combat`) с полем `public StatusEffectSystem statusEffects`. В `HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)`: если `module`/`statusEffects`/`target` null — выйти; иначе `statusEffects.ApplyBurn(target, module.tickDamage, module.tickRate, module.duration)`.

## Ограничения
- Логику тиков не дублировать — только делегирование в `StatusEffectSystem`.
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` (OnHitModuleData), `using TankSurvival.Enemy` (EnemyController), `using UnityEngine` (Vector3).

## Unity (человеку)
1. В сцене создать пустой GameObject `BurnOnHitHandler` → добавить компонент `BurnOnHitHandler`.
2. `BurnOnHitHandler.statusEffects` → объект `StatusEffectSystem` (из T031).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
