# T088: Dev Menu — экран перед раундом (показ при старте + Start Round)
- Status: [ ]
- Phase: 10.8
- Files:
  - Assets/Scripts/Core/GameBootstrap.cs
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Превратить Dev Menu из оверлея (F1/`~`) в экран настроек, показываемый при входе в Play Mode: раунд не стартует автоматически в dev-сборке, а запускается кнопкой «Start Round».

`Assets/Scripts/Core/GameBootstrap.cs`:
1. В методе `Start()` обернуть только строку `roundController.StartRound();` в директиву:
   ```csharp
   #if !UNITY_EDITOR && !DEBUG
       roundController.StartRound();
   #endif
   ```
   (`poolManager.Prewarm(...)`, `poolManager.Prewarm(...)` и `waveSpawner.Prewarm()` остаются вне директивы.)

`Assets/Scripts/Dev/DevMenuController.cs` (namespace `TankSurvival.Dev`):
2. Добавить поле `private RoundController _roundController;` и `using TankSurvival.Round;`.
3. В `Awake()`, после `BuildPanel();`, добавить `_roundController = Object.FindFirstObjectByType<RoundController>();` и заменить `SetVisible(false);` на `SetVisible(true);`.
4. Удалить метод `Update()` целиком (тогл F1/`~` больше не нужен) и удалить `using UnityEngine.InputSystem;` (Keyboard больше не используется).
5. В `BuildPanel()` удалить создание кнопки закрытия (блок `Button close = CreateButton("Close", _panel, "X");` … `close.onClick.AddListener(ClosePanel);`) и удалить метод `ClosePanel()`.
6. Заголовок: заменить строку `CreateText("Title", _panel, "DEV MENU  [F1 / ~]");` на `CreateText("Title", _panel, "DEV MENU");`.
7. В `BuildScroll()`, в блоке настройки `ScrollRect`, заменить `scrollRect.offsetMax = new Vector2(-8f, -40f);` на `scrollRect.offsetMax = new Vector2(-8f, -48f);` (освободить место снизу под панель кнопок).
8. В `BuildPanel()`, после `BuildRows(content);`, добавить нижнюю кнопку «Start Round»:
   ```csharp
   Button start = CreateButton("StartRound", _panel, "Start Round");
   RectTransform startRect = (RectTransform)start.transform;
   startRect.anchorMin = new Vector2(1f, 0f);
   startRect.anchorMax = new Vector2(1f, 0f);
   startRect.pivot = new Vector2(1f, 0f);
   startRect.anchoredPosition = new Vector2(-8f, 8f);
   startRect.sizeDelta = new Vector2(150f, 32f);
   start.onClick.AddListener(StartRound);
   ```
9. Добавить метод:
   ```csharp
   private void StartRound()
   {
       if (_roundController != null)
           _roundController.StartRound();
       SetVisible(false);
   }
   ```

## Ограничения
- `SetVisible`/`BuildPanel`/`BuildScroll`/`BuildRows` остаются; меняется только то, что перечислено.
- Не менять слайдеры, форматирование значений и скролл (это T089–T091).
- Dev Menu по-прежнему строится кодом один раз в `Awake`, в `#if UNITY_EDITOR || DEBUG`.
- В релизе (`!UNITY_EDITOR && !DEBUG`) `GameBootstrap` по-прежнему автостартует раунд; `DevMenuController` в релиз не компилируется.

## Unity (человеку)
Не требуется (панель и кнопка создаются кодом автоматически).

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode (Editor или DEBUG): сразу видна панель Dev Menu, заголовок «DEV MENU», кнопки «X» нет, внизу кнопка «Start Round».
- Раунд НЕ начат автоматически: враги не спавнятся; `RoundController` → `Is Round Active` = false, `Current Wave` = 0.
- Клик «Start Round» → панель скрывается, раунд стартует: `Is Round Active` = true, `Current Wave` = 1, враги спавнятся.
- Клавиши F1 и `~` не открывают и не закрывают панель.

## Отчёт Исполнителя
Готово:
Блокер:
