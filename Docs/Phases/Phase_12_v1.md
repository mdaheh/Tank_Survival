# Phase 12: Стрельба v2 — фундамент lifecycle
- Status: accepted
- Цель: ввести изолированный слой lifecycle-модели выстрела из `Docs/Shooting.md`
  (контракты стадий, `ShotContext`, `BuildSnapshot`/`SpawnRequest`/`Target`,
  enum-ы, `IVfxSpawner`), не меняя поведение текущей стрельбы.

## Задачи (порядок = приоритет)
- T112: enum `ShotState` / `DestroyReason` / `DeliveryType`.
- T113: `SpawnRequest` + `Target` (данные спавна и цели).
- T114: контракты стадий `IOnSpawn`…`IOnDestroy` + `IResettable`.
- T115: `BuildSnapshot` (модули по стадиям + базовые статы, `Without<T>()`).
- T116: `ShotContext` (состояние выстрела, `Reset()`).
- T117: `IVfxSpawner` (интерфейс спавна косметики).

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] Текущая стрельба (Hammer/Beam/Laser/Cone + on-hit) работает как до фазы.
- [x] В `Assets/Scripts/Combat/Shooting/` появились файлы `ShotTypes.cs`,
      `SpawnRequest.cs`, `Target.cs`, `ShotContracts.cs`, `BuildSnapshot.cs`,
      `ShotContext.cs`, `IVfxSpawner.cs` с объявленными типами и полями.
