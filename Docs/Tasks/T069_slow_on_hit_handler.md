# T069: SlowOnHitHandler
- Status: [ ]
- Phase: 10
- Depends: T066, T068
- Files:
  - Assets/Scripts/Combat/SlowOnHitHandler.cs

## Задача
Создать `SlowOnHitHandler : MonoBehaviour, IHitHandler` (namespace `TankSurvival.Combat`): поле `public ControlEffectSystem controlEffects`. В `HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)`: если `module == null || controlEffects == null || target == null` — выйти; иначе `controlEffects.ApplySlow(target, module.slowAmount, module.duration)`.

## Ограничения
- Только делегирование в `ControlEffectSystem`. namespace `TankSurvival.Combat`; `using TankSurvival.Data` (OnHitModuleData), `using TankSurvival.Enemy` (EnemyController), `using UnityEngine`.

## Unity (человеку)
1. В сцене создать пустой GameObject `SlowOnHitHandler` → добавить компонент `SlowOnHitHandler`; поле `controlEffects` → объект `ControlEffectSystem`.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
