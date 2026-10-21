# T075: GameServices — service locator базовых систем
- Status: [ ]
- Phase: 10.5
- Files:
  - Assets/Scripts/Core/GameServices.cs
  - Assets/Scripts/Core/PoolManager.cs
  - Assets/Scripts/Combat/StatusEffectSystem.cs
  - Assets/Scripts/Combat/ControlEffectSystem.cs
  - Assets/Scripts/Combat/OnHitDispatcher.cs

## Задача
Создать статический локатор `GameServices` (namespace `TankSurvival.Core`) с методами
`Register<T>(T instance)`, `TryGet<T>(out T instance)` и `Get<T>()`, хранящий инстансы
по типу в `Dictionary<Type, object>`. В `Awake()` четырёх базовых систем добавить
саморегистрацию `GameServices.Register(this)`: `PoolManager`, `StatusEffectSystem`,
`ControlEffectSystem`, `OnHitDispatcher`.

API:
- `Register<T>(T instance)` — кладёт `instance` в словарь по ключу `typeof(T)`.
- `TryGet<T>(out T instance)` — возвращает `true` и инстанс, если тип зарегистрирован.
- `Get<T>()` — возвращает зарегистрированный инстанс или `null` (потребитель сам делает null-проверку).

В `StatusEffectSystem.cs`, `ControlEffectSystem.cs`, `OnHitDispatcher.cs` (namespace
`TankSurvival.Combat`) добавить `using TankSurvival.Core;` и метод `Awake()` с одной
строкой `GameServices.Register(this);`. В `PoolManager.cs` (уже в `TankSurvival.Core`)
только добавить `Awake()` с той же строкой.

## Ограничения
- Никаких аллокаций в `Update`: регистрация — только в `Awake`, `Get` — только `Dictionary`-lookup.
- Не менять существующие публичные поля/API сервисов и хендлеров (миграция — T076).
- `GameServices` — продакшн-код (без `#if`): он нужен в релизе.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode запускается; Slow/Freeze/Burn/Explosion/Ground работают как в Phase 10
  (хендлеры всё ещё используют ссылки из Inspector — поведение не меняется).

## Отчёт Исполнителя
Готово:
Блокер:
