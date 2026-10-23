# T095: Dev Menu — кнопка Reset (сброс параметров)
- Status: [ ]
- Phase: 10.8
- Depends: T090
- Files:
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Добавить в Dev Menu кнопку «Reset», возвращающую все параметры к исходным значениям (с диска) через `DevParam.Restore()` и обновляющую UI строк.

`Assets/Scripts/Dev/DevMenuController.cs`:
1. Добавить поле для ссылок на UI строк:
   ```csharp
   private readonly List<DevParamRow> _rows = new List<DevParamRow>();
   ```
   и вложенный тип (в класс `DevMenuController`):
   ```csharp
   private sealed class DevParamRow
   {
       public DevParam Parameter;
       public Slider Slider;
       public InputField Input;
   }
   ```
2. В `CreateRow`, после создания `slider` и `inputField` (и их подписок), добавить:
   ```csharp
   _rows.Add(new DevParamRow { Parameter = parameter, Slider = slider, Input = inputField });
   ```
3. В `BuildPanel()`, рядом с кнопкой «Start Round» (T088), добавить кнопку «Reset» слева внизу:
   ```csharp
   Button reset = CreateButton("Reset", _panel, "Reset");
   RectTransform resetRect = (RectTransform)reset.transform;
   resetRect.anchorMin = new Vector2(0f, 0f);
   resetRect.anchorMax = new Vector2(0f, 0f);
   resetRect.pivot = new Vector2(0f, 0f);
   resetRect.anchoredPosition = new Vector2(8f, 8f);
   resetRect.sizeDelta = new Vector2(120f, 32f);
   reset.onClick.AddListener(ResetAll);
   ```
4. Добавить метод:
   ```csharp
   private void ResetAll()
   {
       for (int i = 0; i < _rows.Count; i++)
       {
           DevParamRow row = _rows[i];
           row.Parameter.Restore();
           row.Slider.SetValueWithoutNotify(Mathf.Clamp(row.Parameter.GetValue(), row.Slider.minValue, row.Slider.maxValue));
           row.Input.SetTextWithoutNotify(FormatValue(row.Parameter));
       }
   }
   ```

## Ограничения
- Сброс — через `DevParam.Restore()` (возвращает значение с диска), не через пересоздание панели и не через изменение `SessionState`.
- Обновление UI — через `SetValueWithoutNotify`/`SetTextWithoutNotify`, чтобы не срабатывали `onValueChanged`/`onEndEdit`.
- `using System.Collections.Generic;` уже есть в файле (используется в `BuildRows`).

## Unity (человеку)
Не требуется (кнопка создаётся кодом).

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu: изменить несколько параметров (например, `pelletCount` = 25, `speed` = 3.75) → клик «Reset» → все поля вернулись к исходным значениям с диска, слайдеры в исходных позициях.
- После Reset → «Start Round» → игра идёт с исходными (не изменёнными) значениями.

## Отчёт Исполнителя
Готово:
Блокер:
