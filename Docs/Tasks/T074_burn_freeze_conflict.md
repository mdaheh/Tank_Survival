# T074: Конфликт burn ↔ freeze
- Status: [ ]
- Phase: 10
- Depends: T068, T070
- Files:
  - Assets/Scripts/Combat/StatusEffectSystem.cs
  - Assets/Scripts/Combat/FreezeOnHitHandler.cs
  - Assets/Scripts/Combat/BurnOnHitHandler.cs

## Задача
Element group {burn, freeze}: активен один.
1) В `StatusEffectSystem` добавить `public void RemoveBurn(EnemyController enemy)` — найти burn-статус врага и удалить из списка (без аллокаций, без урона).
2) В `FreezeOnHitHandler` добавить поле `public StatusEffectSystem statusEffects` и перестроить `HandleHit`:
   - `if (module == null || target == null) return;`
   - `if (statusEffects != null) statusEffects.RemoveBurn(target);`
   - `if (controlEffects == null) return; controlEffects.ApplyFreeze(target, module.slowAmount, module.duration, module.freezeThreshold);`
3) В `BurnOnHitHandler` добавить поле `public ControlEffectSystem controlEffects` и перестроить `HandleHit`:
   - `if (module == null || target == null) return;`
   - `if (controlEffects != null) controlEffects.RemoveFreeze(target);`
   - `if (statusEffects == null) return; statusEffects.ApplyBurn(target, module.tickDamage, module.tickRate, module.duration);`

## Ограничения
- `RemoveBurn`/`RemoveFreeze` — удаление статуса без нанесения урона и без сброса HP. Без `new`/LINQ.
- `StatusEffectSystem` — только добавление `RemoveBurn`, тики burn не менять. `ControlEffectSystem` не менять (`RemoveFreeze` уже есть из T068).

## Unity (человеку)
1. `FreezeOnHitHandler` (в сцене): поле `statusEffects` → объект `StatusEffectSystem`.
2. `BurnOnHitHandler` (в сцене): поле `controlEffects` → объект `ControlEffectSystem`.

## Проверка человеком
- Проект компилируется без ошибок.
- `FreezeOnHitHandler.statusEffects` = StatusEffectSystem; `BurnOnHitHandler.controlEffects` = ControlEffectSystem.

## Отчёт Исполнителя
Готово:
Блокер:
