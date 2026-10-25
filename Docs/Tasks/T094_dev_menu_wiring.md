# T094: Dev Menu — связка с паузой и RoundEnd
- Status: [x]
- Phase: 10.8
- Depends: T092, T093
- Files:
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Показать Dev Menu после выхода из паузы («Выйти») и после завершения раунда (кнопка RoundEnd) через подписку на события `PauseMenuController.OnExitToMenu` и `RoundEndUI.OnContinue`.

`Assets/Scripts/Dev/DevMenuController.cs`:
1. Добавить `using TankSurvival.UI;`.
2. В `Awake()`, после резолва `_roundController`, добавить:
   ```csharp
   PauseMenuController pause = Object.FindFirstObjectByType<PauseMenuController>();
   if (pause != null)
       pause.OnExitToMenu += ShowDevMenu;

   RoundEndUI roundEnd = Object.FindFirstObjectByType<RoundEndUI>();
   if (roundEnd != null)
       roundEnd.OnContinue += ShowDevMenu;
   ```
3. Добавить метод:
   ```csharp
   private void ShowDevMenu()
   {
       SetVisible(true);
   }
   ```

## Ограничения
- Только подписка и метод показа; не менять логику паузы (T092), RoundEndUI (T093) и содержимое Dev Menu (T088–T091).
- `Object.FindFirstObjectByType<T>()` вызывается один раз в `Awake`.
- Файл в `#if UNITY_EDITOR || DEBUG` — в релиз подписка не компилируется.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → «Start Round» → Esc → «Выйти» → появляется Dev Menu (панель настроек), раунд остановлен (`Is Round Active` = false).
- Play Mode → «Start Round» → довести до победы/поражения → клик по кнопке RoundEnd → появляется Dev Menu (новый раунд не стартует автоматически).
- Из Dev Menu «Start Round» → начинается новый раунд с текущими настройками.

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DevMenuController.cs: добавлен `using TankSurvival.UI;`; в `Awake()` после резолва `_roundController` добавлены подписки `pause.OnExitToMenu += ShowDevMenu` и `roundEnd.OnContinue += ShowDevMenu` (через `Object.FindFirstObjectByType<T>()` с null-guard); добавлен метод `ShowDevMenu()` → `SetVisible(true)`.
Блокер: —
