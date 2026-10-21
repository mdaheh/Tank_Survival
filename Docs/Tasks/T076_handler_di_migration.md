# T076: Миграция on-hit-хендлеров на GameServices
- Status: [ ]
- Phase: 10.5
- Depends: T075
- Files:
  - Assets/Scripts/Combat/SlowOnHitHandler.cs
  - Assets/Scripts/Combat/FreezeOnHitHandler.cs
  - Assets/Scripts/Combat/BurnOnHitHandler.cs
  - Assets/Scripts/Combat/ExplosionOnHitHandler.cs
  - Assets/Scripts/Combat/GroundOnHitHandler.cs

## Задача
Убрать ручную проводку сервисов: публичные поля-ссылки на базовые системы заменить
приватным кэшируемым полем, заполняемым лениво из `GameServices` при первом обращении.
Паттерн для каждого сервиса:

```csharp
private ControlEffectSystem ControlEffects =>
    _controlEffects != null ? _controlEffects : (_controlEffects = GameServices.Get<ControlEffectSystem>());
```

Маппинг полей (публичное поле удаляется → приватный кэш + ленивое разрешение):
- `SlowOnHitHandler.controlEffects` → `GameServices.Get<ControlEffectSystem>()`.
- `FreezeOnHitHandler.controlEffects` → `ControlEffectSystem`; `FreezeOnHitHandler.statusEffects` → `GameServices.Get<StatusEffectSystem>()`.
- `BurnOnHitHandler.statusEffects` → `StatusEffectSystem`; `BurnOnHitHandler.controlEffects` → `ControlEffectSystem`.
- `ExplosionOnHitHandler.pool` → `GameServices.Get<PoolManager>()`.
- `GroundOnHitHandler.pool` → `PoolManager`; `GroundOnHitHandler.onHit` → `GameServices.Get<OnHitDispatcher>()`.

Обновить все обращения в `HandleHit` на новые свойства; сохранить существующие null-проверки.
В `SlowOnHitHandler.cs`, `FreezeOnHitHandler.cs`, `BurnOnHitHandler.cs` добавить
`using TankSurvival.Core;` (в `ExplosionOnHitHandler.cs` и `GroundOnHitHandler.cs` он уже есть).

## Ограничения
- Не менять логику `HandleHit` и сигнатуры (только источник получения сервисов).
- Не трогать `IHitHandler`, `OnHitDispatcher`, `DeliveryHandler` и интерфейсы.
- Публичные поля-сервисы удалить полностью (не оставлять как fallback — цель убрать проводку).
- Ленивое разрешение — не в `Update`; после первого обращения поле кэшировано, `Dictionary`-lookup один раз.

## Unity (человеку)
Не требуется (поля исчезнут из Inspector сами после компиляции).

## Проверка человеком
- В Inspector хендлеров (`SlowOnHitHandler`, `FreezeOnHitHandler`, `BurnOnHitHandler`,
  `ExplosionOnHitHandler`, `GroundOnHitHandler`) больше нет полей
  `controlEffects`/`statusEffects`/`pool`/`onHit`.
- Play Mode: Slow/Freeze/Burn/Explosion/Ground работают как в Phase 10 (поведение не изменилось).

## Отчёт Исполнителя
Готово:
Блокер:
