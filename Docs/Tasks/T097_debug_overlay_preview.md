# T097: Debug Overlay — preview второго врага (панель справа)
- Status: [ ]
- Phase: 10.9
- Depends: T096
- Files:
  - Assets/Scripts/Dev/DebugOverlayController.cs

## Задача
Добавить вторую панель справа от основной: при закреплённом враге (pin) hover на другого врага показывает его display в preview-панели, не перекрывая основную.

`Assets/Scripts/Dev/DebugOverlayController.cs`:

1. Добавить поля:
```csharp
private readonly List<DevParam> _previewParams = new List<DevParam>();
private readonly List<DevParam> _previewHealthParams = new List<DevParam>();

private RectTransform _previewPanel;
private Text _previewText;
private EnemyController _observedPreviewEnemy;
```

2. В `BuildOverlay()`, после создания основной панели (после `_statusText = CreateText("Statuses", panel);`), добавить создание preview-панели:
```csharp
RectTransform previewPanel = CreateRect("PreviewPanel", transform);
previewPanel.anchorMin = new Vector2(0f, 1f);
previewPanel.anchorMax = new Vector2(0f, 1f);
previewPanel.pivot = new Vector2(0f, 1f);
previewPanel.anchoredPosition = new Vector2(12f + OverlayWidth + 8f, -12f);
previewPanel.sizeDelta = new Vector2(OverlayWidth, 0f);

Image previewBackground = previewPanel.gameObject.AddComponent<Image>();
previewBackground.color = new Color(0.05f, 0.08f, 0.12f, 0.6f);

VerticalLayoutGroup previewLayout = previewPanel.gameObject.AddComponent<VerticalLayoutGroup>();
previewLayout.childControlWidth = true;
previewLayout.childControlHeight = true;
previewLayout.childForceExpandWidth = true;
previewLayout.childForceExpandHeight = false;
previewLayout.spacing = 4f;
previewLayout.padding = new RectOffset(8, 8, 8, 8);

ContentSizeFitter previewFitter = previewPanel.gameObject.AddComponent<ContentSizeFitter>();
previewFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
previewFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

_previewText = CreateText("Preview", previewPanel);

_previewPanel = previewPanel;
_previewPanel.gameObject.SetActive(false);
```

3. Заменить `BuildEnemyText()` на общий метод (используется и для основного блока, и для preview):
```csharp
private string BuildEnemyBlock(string header, EnemyController enemy, List<DevParam> enemyParams, List<DevParam> healthParams)
{
    _builder.Clear();
    _builder.Append("<b>").Append(header).Append("</b>\n");

    if (enemy == null)
    {
        _builder.Append("нет врага под курсором\n");
        return _builder.ToString();
    }

    AppendParams(enemyParams);
    AppendParams(healthParams);
    _builder.Append("Дистанция: ");
    _builder.Append(DistanceToPlayer(enemy).ToString("0.00"));
    _builder.Append('\n');

    return _builder.ToString();
}
```

4. Добавить `PreviewEnemy()` и `ObservePreviewEnemy(...)`:
```csharp
private EnemyController PreviewEnemy()
{
    if (_pinnedEnemy == null || _hoveredEnemy == null || _hoveredEnemy == _pinnedEnemy)
        return null;

    return _hoveredEnemy;
}

private void ObservePreviewEnemy(EnemyController enemy)
{
    if (enemy == _observedPreviewEnemy)
        return;

    _observedPreviewEnemy = enemy;

    _previewParams.Clear();
    _previewParams.AddRange(DevParamRegistry.Reflect(enemy, "Enemy"));

    _previewHealthParams.Clear();
    _previewHealthParams.AddRange(DevParamRegistry.Reflect(enemy != null ? enemy.health : null, "Enemy.Health"));
}
```

5. Переписать `Refresh()`:
```csharp
private void Refresh()
{
    EnemyController main = MainEnemy();
    EnemyController preview = PreviewEnemy();

    ObserveEnemy(main);
    ObservePreviewEnemy(preview);

    _playerText.text = BuildPlayerText();
    _enemyText.text = BuildEnemyBlock("Враг", main, _enemyParams, _enemyHealthParams);
    _previewText.text = BuildEnemyBlock("Враг (preview)", preview, _previewParams, _previewHealthParams);
    _statusText.text = BuildStatusText();

    _previewPanel.gameObject.SetActive(preview != null);
}
```

## Ограничения
- Preview-панель показывается только когда есть pin И hover на другого врага (`_hoveredEnemy != _pinnedEnemy`).
- `BuildEnemyBlock` использует общий `_builder` — порядок присвоения `.text` важен (player → enemy → preview → statuses), т.к. `StringBuilder.ToString()` возвращает новую строку.
- Позиция preview: по X `12 + OverlayWidth + 8 = 380`, по Y `-12` — справа от основной панели, не перекрывает её.
- Не добавлять GraphicRaycaster в этой задаче (появится в T100).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Start Round → клик по врагу (pin) → навести на другого врага → справа появилась панель «Враг (preview)» с его `currentHealth` и «Дистанция», основная панель не перекрыта.
- Убрать мышь (не на враге) → preview-панель скрылась, основная осталась (закреплена).
- Без pin (клик в пустоту) → hover на враге показывает его только в основной панели, preview не появляется.

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DebugOverlayController.cs: добавлены поля `_previewParams/_previewHealthParams/_previewPanel/_previewText/_observedPreviewEnemy`; в `BuildOverlay` создана preview-панель (справа от основной, скрыта по умолчанию); `BuildEnemyText` заменён на общий `BuildEnemyBlock(header, enemy, enemyParams, healthParams)`; добавлены `PreviewEnemy()` и `ObservePreviewEnemy(...)`; `Refresh()` заполняет обе панели и управляет видимостью preview.
Блокер: —
