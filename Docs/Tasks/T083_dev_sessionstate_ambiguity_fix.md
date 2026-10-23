# T083: DevParamRegistry — устранить неоднозначность SessionState (CS0104)
- Status: [x]
- Phase: 10.6
- Depends: T082
- Files:
  - Assets/Scripts/Dev/DevParamRegistry.cs

## Задача
В `Assets/Scripts/Dev/DevParamRegistry.cs` строка 22 не компилируется (CS0104): тип `SessionState` неоднозначен между `TankSurvival.Core.SessionState` и `UnityEditor.SessionState` (коллизия из-за добавленного в T082 `using UnityEditor;`). Заменить строку 22 на полное имя типа:

`TankSurvival.Core.SessionState session = Object.FindAnyObjectByType<TankSurvival.Core.SessionState>();`

Больше ничего в файле не менять.

## Ограничения
- Менять только строку 22 в `DevParamRegistry.cs`.
- Не трогать `using UnityEditor;`, хук `playModeStateChanged` и файл `DevParam.cs` — они корректны.
- Не менять `ProjectSettings/`, `Packages/`, `manifest.json`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок: CS0104 исчез; Burst-сообщения от URP (шум при неудачной компиляции) уходят после успешной сборки.
- Повторить smoke-тест T082: правка `speed` в Dev Menu → Stop → повторный вход в Play Mode → слайдер снова равен значению с диска.

## Отчёт Исполнителя
Готово: DevParamRegistry.cs: строка 22 — `TankSurvival.Core.SessionState session = Object.FindAnyObjectByType<TankSurvival.Core.SessionState>();` (полное имя типа, CS0104 устранён).
Блокер: —
