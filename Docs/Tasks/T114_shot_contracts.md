# T114: ShotContracts — контракты стадий выстрела
- Status: [x]
- Phase: 12
- Depends: T113
- Files:
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs

## Задача
Создать `Assets/Scripts/Combat/Shooting/ShotContracts.cs` (namespace
`TankSurvival.Combat.Shooting`) с интерфейсами модулей выстрела (сигнатуры из
Shooting.md §6, §5):
- `IOnSpawn` — `void OnSpawn(ShotContext ctx);`
- `IOnFlight` — `void OnFlight(ShotContext ctx, float dt);`
- `IOnHit` — `void OnHit(ShotContext ctx, Target target);`
- `IOnPostHit` — `void OnPostHit(ShotContext ctx);`
- `IOnDestroy` — `void OnDestroy(ShotContext ctx);`
- `IResettable` — `void ResetState();` (сброс кастомных полей контекста, §5).

## Ограничения
- Только сигнатуры, без реализаций. `ShotContext` объявляется в T116 (forward-ссылка допустима).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/Shooting/ShotContracts.cs — интерфейсы IOnSpawn (OnSpawn), IOnFlight (OnFlight(ctx, dt)), IOnHit (OnHit(ctx, target)), IOnPostHit (OnPostHit), IOnDestroy (OnDestroy), IResettable (ResetState) без реализаций.
Блокер: —
