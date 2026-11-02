# T102: GameServices — сброс статического реестра между сессиями
- Status: [x]
- Phase: 10.10
- Files:
  - Assets/Scripts/Core/GameServices.cs

## Задача
Добавить в `GameServices` публичный метод `Clear()` (очищает `_services`) и явную точку сброса статики между Play Mode-сессиями: в редакторе при выходе из Play Mode вызывать `Clear()`, чтобы на входе в следующую сессию реестр был пуст и сервисы заново регистрировались в `Awake` (без fake-null ссылок на уничтоженные объекты).

В `Assets/Scripts/Core/GameServices.cs`:

1. После существующих `using System;` / `using System.Collections.Generic;` добавить:
```csharp
#if UNITY_EDITOR
using UnityEditor;
#endif
```

2. В класс `GameServices` (после метода `Get<T>()`) добавить:
```csharp
public static void Clear()
{
    _services.Clear();
}
```

3. В конец класса (перед закрывающей скобкой класса) добавить editor-only сброс по образцу `DevParamRegistry`:
```csharp
#if UNITY_EDITOR
    private static bool _playModeHookInstalled;

    [InitializeOnLoadMethod]
    private static void InstallPlayModeReset()
    {
        if (_playModeHookInstalled)
        {
            return;
        }

        _playModeHookInstalled = true;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Clear();
        }
    }
#endif
```

## Ограничения
- `Clear()` срабатывает на `ExitingPlayMode` (конец предыдущей сессии), поэтому на входе в новую сессию `_services` пуст, и `Awake` всех систем (`PoolManager`, `StatusEffectSystem`, `ControlEffectSystem`, `OnHitDispatcher`) заново вызывает `GameServices.Register(this)`.
- `EnemyController.Active` в этой задаче не трогаем: реестр очищается в `OnEnable`/`OnDisable` (Architecture.md), отдельного сброса не требует.
- `DevParamRegistry.All` сбрасывается в `Init()` — не трогаем.
- Editor-блок изолирован `#if UNITY_EDITOR` и не попадает в WebGL-сборку. `Clear()` остаётся публичным рантайм-API.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок (включая editor-сборку).
- Play Mode → Start Round (бой работает) → выйти из Play Mode → снова Play Mode → бой работает с первого запуска; в Console нет NRE / MissingReferenceException про отсутствующие сервисы.
- При использовании «Enter Play Mode Options → Reload Domain» выключенным — повторные входы не дают stale-состояний.

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/GameServices.cs: добавлен editor-only `using UnityEditor` под `#if UNITY_EDITOR`, публичный `Clear()` (очистка `_services`) и `#if UNITY_EDITOR`-блок с `[InitializeOnLoadMethod] InstallPlayModeReset()` + `OnPlayModeStateChanged`, вызывающий `Clear()` на `ExitingPlayMode`.
Блокер: —
