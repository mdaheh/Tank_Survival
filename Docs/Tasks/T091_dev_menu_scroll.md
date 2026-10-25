# T091: Dev Menu — исправление скролла списка
- Status: [x]
- Phase: 10.8
- Files:
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Исправить баг: колесо мыши прокручивает список Dev Menu только когда курсор над слайдером. Причина — у объекта `ScrollRect` нет собственного raycast-графика, поэтому события колеса ловятся только графиками слайдеров.

`Assets/Scripts/Dev/DevMenuController.cs`, в методе `BuildScroll` (после блока настройки `ScrollRect`, перед `return content;`) добавить фоновый `Image` на сам объект ScrollRect:
```csharp
Image scrollBackground = scrollRect.gameObject.AddComponent<Image>();
scrollBackground.color = new Color(0.08f, 0.09f, 0.12f, 1f);
```

## Ограничения
- Фон непрозрачный (alpha = 1) — прозрачный график (`alpha < 0.005`) не участвует в raycast колеса мыши.
- Не менять `viewport`, `content`, `VerticalLayoutGroup`, `ContentSizeFitter` и остальную структуру скролла.
- Не менять слайдеры/InputField (T090) и секции (T089).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu, список длиннее видимой области: колесо мыши прокручивает список, когда курсор находится над текстом названия параметра и над пустым местом строки (не только над слайдером).

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DevMenuController.cs: в `BuildScroll` после блока настройки `ScrollRect` перед `return content;` добавлен фоновый `Image` на объект ScrollRect с цветом `new Color(0.08f, 0.09f, 0.12f, 1f)`.
Блокер: —
