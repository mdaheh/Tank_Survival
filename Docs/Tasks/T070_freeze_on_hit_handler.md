# T070: FreezeOnHitHandler
- Status: [x]
- Phase: 10
- Depends: T066, T068
- Files:
  - Assets/Scripts/Combat/FreezeOnHitHandler.cs

## Задача
Создать `FreezeOnHitHandler : MonoBehaviour, IHitHandler` (namespace `TankSurvival.Combat`): поле `public ControlEffectSystem controlEffects`. В `HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)`: если `module == null || controlEffects == null || target == null` — выйти; иначе `controlEffects.ApplyFreeze(target, module.slowAmount, module.duration, module.freezeThreshold)`.

## Ограничения
- Только делегирование в `ControlEffectSystem`. Конфликт с burn добавляется в T074 (не здесь). namespace `TankSurvival.Combat`; `using TankSurvival.Data`, `using TankSurvival.Enemy`, `using UnityEngine`.

## Unity (человеку)
1. В сцене создать пустой GameObject `FreezeOnHitHandler` → добавить компонент `FreezeOnHitHandler`; поле `controlEffects` → объект `ControlEffectSystem`.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/FreezeOnHitHandler.cs: создан MonoBehaviour с интерфейсом IHitHandler и полем controlEffects; HandleHit с guard'ами делегирует ApplyFreeze(target, module.slowAmount, module.duration, module.freezeThreshold).
Блокер: —
