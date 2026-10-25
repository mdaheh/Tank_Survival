# T100: Debug Overlay — крестик (✕) для открепления
- Status: [ ]
- Phase: 10.9
- Depends: T096
- Files:
  - Assets/Scripts/Dev/DebugOverlayController.cs

## Задача
Добавить кнопку «✕» в правом верхнем углу основной панели оверлея: клик открепляет закреплённого врага. Показывается только при закреплении.

`Assets/Scripts/Dev/DebugOverlayController.cs`:

1. В `CreateOverlay()`, после `overlay.AddComponent<DebugOverlayController>();`, добавить GraphicRaycaster (иначе Button не получит клик):
```csharp
overlay.AddComponent<GraphicRaycaster>();
```

2. В `BuildOverlay()`:
   a. У основной панели после `background.color = new Color(0f, 0f, 0f, 0.55f);` отключить перехват кликов фоном:
```csharp
background.raycastTarget = false;
```
   b. У preview-панели (переменная `previewBackground` из T097) после `previewBackground.color = ...`:
```csharp
previewBackground.raycastTarget = false;
```
   c. В конце `BuildOverlay()` создать кнопку закрытия и скрыть:
```csharp
_closeButton = CreateCloseButton(panel);
_closeButton.gameObject.SetActive(false);
```

3. Добавить поле:
```csharp
private Button _closeButton;
```

4. Добавить методы:
```csharp
private Button CreateCloseButton(RectTransform parent)
{
    RectTransform rect = CreateRect("CloseButton", parent);

    LayoutElement ignore = rect.gameObject.AddComponent<LayoutElement>();
    ignore.ignoreLayout = true;

    rect.anchorMin = new Vector2(1f, 1f);
    rect.anchorMax = new Vector2(1f, 1f);
    rect.pivot = new Vector2(1f, 1f);
    rect.anchoredPosition = new Vector2(-4f, -4f);
    rect.sizeDelta = new Vector2(20f, 20f);

    Image background = rect.gameObject.AddComponent<Image>();
    background.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
    background.raycastTarget = true;

    Text label = CreateText("X", rect);
    label.text = "✕";
    label.fontSize = 12;
    label.alignment = TextAnchor.MiddleCenter;
    label.raycastTarget = false;
    label.rectTransform.anchorMin = Vector2.zero;
    label.rectTransform.anchorMax = Vector2.one;
    label.rectTransform.offsetMin = Vector2.zero;
    label.rectTransform.offsetMax = Vector2.zero;

    Button button = rect.gameObject.AddComponent<Button>();
    button.targetGraphic = background;
    button.onClick.AddListener(Unpin);

    return button;
}

private void Unpin()
{
    _pinnedEnemy = null;
}
```

5. В `Refresh()` добавить управление видимостью крестика (например, после строки с `_previewPanel.gameObject.SetActive(...)`):
```csharp
_closeButton.gameObject.SetActive(_pinnedEnemy != null);
```

## Ограничения
- `GraphicRaycaster`/`Button`/`LayoutElement` — из `UnityEngine.UI` (уже импортирован в файле).
- Крестик — ребёнок основной панели с `LayoutElement.ignoreLayout = true`, чтобы не участвовал в `VerticalLayoutGroup`.
- Фон панелей и текст — `raycastTarget = false`, кликабелен только крестик (не блокирует клики по игре).
- EventSystem в сцене уже есть (кнопки HUD/DevMenu/PauseMenu работают); новый не создавать.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Start Round → клик по врагу (pin) → в правом верхнем углу основной панели появился красный крестик «✕».
- Клик по крестику → открепилось (блок «Враг» → «нет врага под курсором», крестик исчез).
- Клики по игре в области панелей не перехватываются (кликабелен только крестик).

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DebugOverlayController.cs: в CreateOverlay добавлен GraphicRaycaster; в BuildOverlay фон основной и preview-панелей raycastTarget=false, в конце создан `_closeButton` (скрыт); добавлены поле `_closeButton`, методы `CreateCloseButton` (кнопка «✕» в правом верхнем углу, ignoreLayout, Unpin по onClick) и `Unpin`; в Refresh управляется видимость крестика по `_pinnedEnemy != null`.
Блокер: —
