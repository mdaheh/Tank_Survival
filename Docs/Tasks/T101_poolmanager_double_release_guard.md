# T101: PoolManager.Release — guard от двойного возврата
- Status: [x]
- Phase: 10.10
- Files:
  - Assets/Scripts/Core/PoolManager.cs

## Задача
В методе `Release(GameObject instance)` класса `PoolManager` добавить guard от повторного возврата уже неактивного (находящегося в пуле) объекта: если `instance` уже выключен — залогировать ошибку и выйти, не помещая объект в очередь пула повторно.

В `Assets/Scripts/Core/PoolManager.cs`, в теле `Release`, сразу после существующей проверки
`if (instance == null || !_instancePrefabs.TryGetValue(instance, out GameObject prefab)) { ... return; }`
добавить:
```csharp
if (!instance.activeSelf)
{
    Debug.LogError("PoolManager: Release got an already-pooled (inactive) instance.", instance);
    return;
}
```

## Ограничения
- Инвариант пула: `Get` всегда активирует объект (`SetActive(true)`), `Release`/`Prewarm` — деактивируют (`SetActive(false)`). Поэтому `activeSelf == false` означает «уже в пуле».
- Guard не меняет обычный жизненный цикл (спавн → смерть → deferred release через `EnemyKillSystem.LateUpdate`).
- В файле уже есть `using UnityEngine;` — `Debug` доступен, новых using не требуется.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Start Round → убить врага: в Console нет сообщения «already-pooled» (обычный release проходит один раз).
- Прогнать несколько волн: пул стабилен, враги спавнятся и умирают без рассинхронизации (нет «призрачных»/задвоенных объектов).

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/PoolManager.cs: в `Release` добавлен guard `if (!instance.activeSelf)` — при повторном возврате уже неактивного (находящегося в пуле) объекта логируется ошибка и выполняется выход без повторного помещения в очередь пула.
Блокер: —
