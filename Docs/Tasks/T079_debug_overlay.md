# T079: DebugOverlayController + dev-аксессоры статусов
- Status: [x]
- Phase: 10.5
- Depends: T077, T075
- Files:
  - Assets/Scripts/Dev/DebugOverlayController.cs
  - Assets/Scripts/Combat/StatusEffectSystem.cs
  - Assets/Scripts/Combat/ControlEffectSystem.cs

## Задача
Создать dev-only Debug Overlay (файл `DebugOverlayController.cs` в `#if UNITY_EDITOR || DEVELOPMENT_BUILD`):
программный Canvas (top-left) поверх игры. Оверлей показывает группы:
1) «Игрок» — `DevParamRegistry.Reflect(statBlock, "Player")` (поля `StatBlock`) + строка
   `PlayerHealth.CurrentHealth` / `StatBlock.maxHealth` (текущее HP — публичное свойство, не поле);
2) «Враг» — ближайший к игроку активный `EnemyController` из `EnemyController.Active`:
   `DevParamRegistry.Reflect(enemy, "Enemy")` + `DevParamRegistry.Reflect(enemy.health, "Enemy.Health")`
   + дистанция до игрока.

Агрегаты (фиксированный блок): `EnemyController.Active.Count`, число активных burn
(`StatusEffectSystem.ActiveBurnCount`), freeze-стаки наблюдаемого врага
(`ControlEffectSystem.GetFreezeStacks(enemy)`) и флаг `IsFrozen`.

Ссылки на `StatBlock`/`PlayerHealth` брать через `Object.FindFirstObjectByType<T>()` (один раз);
`StatusEffectSystem`/`ControlEffectSystem` — через `GameServices.Get<T>()` (T075). Обновление текста —
кэшированным корутином `WaitForSecondsRealtime(0.25f)` (не в `Update`).

Dev-only аксессоры (в `#if UNITY_EDITOR || DEVELOPMENT_BUILD` внутри продакшн-файлов):
- `StatusEffectSystem`: `public int ActiveBurnCount => burns.Count;` и
  `public float GetBurnRemaining(EnemyController enemy)` (остаток длительности текущего burn, иначе 0).
- `ControlEffectSystem`: `public int GetFreezeStacks(EnemyController enemy)` и
  `public bool IsFrozen(EnemyController enemy)` (через существующий `Find`).

## Ограничения
- Весь dev-код — только в `#if UNITY_EDITOR || DEVELOPMENT_BUILD`; аксессоры в продакшн-файлах тоже обёрнуты в `#if`.
- Не менять существующую логику систем (аксессоры только читают состояние).
- Обновление оверлея — по корутину с таймером, не каждый кадр; без `Find*`/LINQ/`GetComponent` в игровом цикле.

## Unity (человеку)
Не требуется (оверлей создаётся автоматически).

## Проверка человеком
- Оверлей показывает числовые HP/скорость/модификаторы игрока и ближайшего врага.
- При наложении Burn/Freeze/Slow на врага значения статусов (burn-остаток, freeze-стаки, frozen)
  меняются в оверлее.
- Новое `public float` поле в `StatBlock`/`EnemyController` появляется в оверлее без правок кода оверлея.

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DebugOverlayController.cs: dev-only оверлей (static [RuntimeInitializeOnLoadMethod(AfterSceneLoad)] создаёт Canvas "DebugOverlay" ScreenSpaceOverlay sortingOrder 90 + CanvasScaler, панель top-left), StatBlock/PlayerHealth берутся Object.FindFirstObjectByType один раз, StatusEffectSystem/ControlEffectSystem — GameServices.Get<T> один раз в Awake, группы «Игрок» (Reflect(statBlock,"Player") + строка HP CurrentHealth/maxHealth), «Враг» (ближайший активный из EnemyController.Active + Reflect(enemy,"Enemy") + Reflect(enemy.health,"Enemy.Health") + дистанция; рефлексия врага перестраивается только при смене наблюдаемого врага) и «Статусы» (Active.Count, ActiveBurnCount, GetBurnRemaining, GetFreezeStacks, IsFrozen), обновление кэшированным WaitForSecondsRealtime(0.25f) с StringBuilder, весь файл в #if UNITY_EDITOR || DEVELOPMENT_BUILD; Assets/Scripts/Combat/StatusEffectSystem.cs: в #if добавлены public int ActiveBurnCount и public float GetBurnRemaining(EnemyController); Assets/Scripts/Combat/ControlEffectSystem.cs: в #if добавлены public int GetFreezeStacks(EnemyController) и public bool IsFrozen(EnemyController) через существующий Find, логика систем не изменена.
Блокер: —
