# T090: Dev Menu — ручной ввод точных чисел + корректное форматирование
- Status: [x]
- Phase: 10.8
- Files:
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Заменить у каждой строки Dev Menu текстовое поле значения на `InputField` (ручной ввод точных чисел, не ограниченный слайдером) и исправить форматирование чисел (int — целые, float — с точкой, без лишних знаков).

`Assets/Scripts/Dev/DevMenuController.cs`:
1. Добавить `using System.Globalization;`.
2. Метод `FormatValue` заменить на:
   ```csharp
   private static string FormatValue(DevParam parameter)
   {
       float value = parameter.GetValue();
       return parameter.IsInteger
           ? Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture)
           : value.ToString("0.###", CultureInfo.InvariantCulture);
   }
   ```
3. Метод `OnSliderChanged(DevParam parameter, float value, Text valueText)` заменить на:
   ```csharp
   private static void OnSliderChanged(DevParam parameter, float value, InputField inputField)
   {
       parameter.SetValue(value);
       inputField.SetTextWithoutNotify(FormatValue(parameter));
   }
   ```
4. В `CreateRow` удалить блок `Text value = CreateText("Value", row, FormatValue(parameter));` … `slider.onValueChanged.AddListener(newValue => OnSliderChanged(parameter, newValue, value));` и вместо него создать `InputField` и подписки:
   ```csharp
   InputField inputField = CreateInputField(row, parameter);
   RectTransform inputRect = (RectTransform)inputField.transform;
   inputRect.anchorMin = new Vector2(0.78f, 0f);
   inputRect.anchorMax = new Vector2(1f, 1f);
   inputRect.offsetMin = new Vector2(6f, 0f);
   inputRect.offsetMax = new Vector2(-8f, 0f);

   slider.onValueChanged.AddListener(newValue => OnSliderChanged(parameter, newValue, inputField));
   inputField.onEndEdit.AddListener(text => OnInputCommitted(parameter, text, slider, inputField));
   ```
5. Добавить метод создания поля ввода:
   ```csharp
   private InputField CreateInputField(RectTransform parent, DevParam parameter)
   {
       RectTransform rect = CreateRect("Input", parent);

       Image background = rect.gameObject.AddComponent<Image>();
       background.color = new Color(0.12f, 0.14f, 0.17f, 1f);

       RectTransform textRect = CreateRect("Text", rect);
       Stretch(textRect, new Vector2(4f, 0f), new Vector2(-4f, 0f));
       Text text = textRect.gameObject.AddComponent<Text>();
       text.font = _font;
       text.fontSize = 14;
       text.color = Color.white;
       text.alignment = TextAnchor.MiddleRight;
       text.horizontalOverflow = HorizontalWrapMode.Overflow;
       text.verticalOverflow = VerticalWrapMode.Overflow;
       text.supportRichText = false;
       text.raycastTarget = true;

       InputField inputField = rect.gameObject.AddComponent<InputField>();
       inputField.textComponent = text;
       inputField.targetGraphic = background;
       inputField.text = FormatValue(parameter);

       return inputField;
   }
   ```
6. Добавить метод фиксации ввода:
   ```csharp
   private static void OnInputCommitted(DevParam parameter, string text, Slider slider, InputField inputField)
   {
       if (parameter.IsInteger)
       {
           if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iv))
               parameter.SetValue(iv);
       }
       else
       {
           if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float fv))
               parameter.SetValue(fv);
       }

       slider.SetValueWithoutNotify(Mathf.Clamp(parameter.GetValue(), slider.minValue, slider.maxValue));
       inputField.SetTextWithoutNotify(FormatValue(parameter));
   }
   ```
   (при неудачном разборе `SetValue` не вызывается, но `SetTextWithoutNotify(FormatValue(...))` откатывает текст к текущему корректному значению.)

## Ограничения
- Текст внутри `InputField` должен быть raycast-целью (`raycastTarget = true`), иначе поле не кликается и не редактируется.
- Использовать `SetValueWithoutNotify`/`SetTextWithoutNotify` при синхронизации «поле → слайдер»/«слайдер → поле», чтобы не вызывать повторно обработчики.
- `NumberStyles`/`CultureInfo` — из `System.Globalization`.
- Не менять секции (T089) и скролл (T091).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu: у каждой строки справа — редактируемое поле (InputField) с числом.
- int-поля показываются целыми (например, `xpReward`, `pelletCount`, `maxTargets`, `waveCount`, `priority`, `freezeThreshold`, `maxDepth`); float — с точкой и без лишних нулей (например, `1.5`, а не `1,500`).
- Клик в поле `pelletCount` (секция Delivery), ввести `25`, снять фокус (Enter/клик мимо) → поле сохраняет `25` (значение записано; слайдер сдвигается к максимуму, но поле хранит точное значение).
- Ввести в float-поле (например, `speed`) значение `3.75` → поле сохраняет `3.75`.
- Ввести нечисловое (например, `abc`) → после потери фокуса текст возвращается к предыдущему корректному значению.

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DevMenuController.cs: добавлен `using System.Globalization;`; `FormatValue` переведён на `CultureInfo.InvariantCulture`; `OnSliderChanged` принимает `InputField` и использует `SetTextWithoutNotify`; в `CreateRow` вместо `Text value` создаётся `InputField` с подписками на `slider.onValueChanged` и `inputField.onEndEdit`; добавлены `CreateInputField(...)` и `OnInputCommitted(...)`.
Блокер: —
