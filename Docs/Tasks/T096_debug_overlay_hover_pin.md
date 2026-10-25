# T096: Debug Overlay — выбор врага под курсором (hover + pin)
- Status: [ ]
- Phase: 10.9
- Depends: —
- Files:
  - Assets/Scripts/Dev/DebugOverlayController.cs

## Задача
Заменить в `DebugOverlayController` автоматический выбор «ближайшего врага» на выбор врага под курсором мыши с закреплением (pin).

`Assets/Scripts/Dev/DebugOverlayController.cs` (namespace `TankSurvival.Dev`, файл целиком в `#if UNITY_EDITOR || DEBUG`):

1. Добавить `using UnityEngine.InputSystem;` в начало блока using (после `using UnityEngine;`).

2. Добавить поля класса:
```csharp
[SerializeField] private float hoverRadius = 2f; // радиус выбора в мировых единицах

private Camera _camera;
private EnemyController _pinnedEnemy;
private EnemyController _hoveredEnemy;
```

3. В `Awake()`, после инициализации `_playerTransform`, добавить получение камеры:
```csharp
_camera = Camera.main != null ? Camera.main : Object.FindAnyObjectByType<Camera>();
```

4. Удалить поле `private float _nearestDistance;` и метод `FindNearestEnemy()`.

5. Добавить метод `Update()` (выполняется каждый кадр — нужно, т.к. `wasPressedThisFrame` истинно только в кадре нажатия):
```csharp
private void Update()
{
    if (_pinnedEnemy != null && !IsAlive(_pinnedEnemy))
        _pinnedEnemy = null;

    _hoveredEnemy = FindEnemyUnderCursor();

    Mouse mouse = Mouse.current;
    if (mouse == null)
        return;

    if (mouse.leftButton.wasPressedThisFrame)
        _pinnedEnemy = _hoveredEnemy;
}
```
(клик в пустоту открепляет автоматически: если врага под курсором нет, `_hoveredEnemy == null` → `_pinnedEnemy = null`).

6. Добавить вспомогательные методы:
```csharp
private static bool IsAlive(EnemyController enemy)
{
    return enemy != null
        && enemy.gameObject.activeSelf
        && enemy.health != null
        && enemy.health.currentHealth > 0f;
}

private EnemyController FindEnemyUnderCursor()
{
    Mouse mouse = Mouse.current;
    if (mouse == null || _camera == null)
        return null;

    Ray ray = _camera.ScreenPointToRay(mouse.position.ReadValue());

    if (Mathf.Abs(ray.direction.y) < 0.0001f)
        return null;

    float t = -ray.origin.y / ray.direction.y;
    if (t < 0f)
        return null;

    Vector3 groundPoint = ray.origin + ray.direction * t;

    EnemyController best = null;
    float bestSqrDistance = hoverRadius * hoverRadius;

    List<EnemyController> active = EnemyController.Active;
    for (int i = 0; i < active.Count; i++)
    {
        EnemyController enemy = active[i];
        if (!IsAlive(enemy))
            continue;

        float dx = enemy.transform.position.x - groundPoint.x;
        float dz = enemy.transform.position.z - groundPoint.z;
        float sqrDistance = dx * dx + dz * dz;
        if (sqrDistance < bestSqrDistance)
        {
            bestSqrDistance = sqrDistance;
            best = enemy;
        }
    }

    return best;
}

private EnemyController MainEnemy()
{
    return _pinnedEnemy != null ? _pinnedEnemy : _hoveredEnemy;
}

private float DistanceToPlayer(EnemyController enemy)
{
    if (enemy == null || _playerTransform == null)
        return 0f;

    return (enemy.transform.position - _playerTransform.position).magnitude;
}
```

7. В `Refresh()` заменить `ObserveEnemy(FindNearestEnemy());` на `ObserveEnemy(MainEnemy());`.

8. В `BuildEnemyText()`:
   - заменить `_builder.Append(_nearestDistance.ToString("0.00"));` на `_builder.Append(DistanceToPlayer(_observedEnemy).ToString("0.00"));`
   - заменить текст пустого состояния `"нет активных врагов"` на `"нет врага под курсором"`.

## Ограничения
- Ввод только через новую Input System (`Mouse.current`) — проект использует `activeInputHandler: 1` (new only); `UnityEngine.Input.mousePosition` бросит исключение в рантайме.
- Выбор врага — проекция курсора на плоскость `y = 0` (`Camera.ScreenPointToRay` + пересечение луча с плоскостью) и ближайший по XZ `sqrMagnitude` из `EnemyController.Active`. Это та же метрика, что в бою (`HitscanDeliveryHandler`/`Projectile` считают попадания по XZ-расстоянию до центра врага), поэтому оверлей подсвечивает того же врага, которого «видит» бой. Коллайдеры (`Capsule Collider` на префабах врагов) не используются — физика в проекте не применяется для hit detection.
- `ScreenPointToRay` + пересечение с `y = 0` работает и для perspective, и для orthographic камеры — не нужно знать проекцию и кэшировать «дистанцию до земли».
- Без аллокаций в `Update` (без `new`/LINQ/`Find*`). `List<EnemyController>` уже импортирован.
- Логика статусов и остальной дисплей в этой задаче не меняется.

## Unity (человеку)
Не требуется (оверлей создаётся автоматически). Требуется камера в сцене (есть, `CameraFollow`); если `Camera.main == null` (нет тега «MainCamera»), оверлей возьмёт её через `Object.FindAnyObjectByType<Camera>()`. Радиус выбора (`hoverRadius`, мировые единицы) правится в Play Mode: Hierarchy → объект `DebugOverlay` → Inspector → `Hover Radius` (по умолчанию `2`).

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu → Start Round: навести мышь на врага → в блоке «Враг» (левый верхний угол) появляются его `currentHealth`, параметры и «Дистанция». Убрать мышь → «нет врага под курсором».
- Клик по врагу → закрепился: увести мышь — блок «Враг» продолжает показывать закреплённого.
- Клик в пустоту → открепился (снова «нет врага под курсором»).
- Закреплённый враг умер → закрепление сброшено автоматически.
- Радиус выбора = `hoverRadius` (по умолчанию 2 мировых единицы); изменить его можно в Play Mode: объект `DebugOverlay` → Inspector → `Hover Radius`.

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DebugOverlayController.cs: добавлены `using UnityEngine.InputSystem;`, поля `[SerializeField] hoverRadius = 2f`, `_camera/_pinnedEnemy/_hoveredEnemy`; в `Awake` получается камера; добавлен `Update()` (валидация `_pinnedEnemy` + `FindEnemyUnderCursor` + обработка `leftButton.wasPressedThisFrame`); удалены `_nearestDistance` и `FindNearestEnemy`; добавлены `IsAlive/FindEnemyUnderCursor/MainEnemy/DistanceToPlayer`; `FindEnemyUnderCursor` считает точку на `y=0` через `ScreenPointToRay` + пересечение луча с плоскостью и выбирает ближайшего по XZ в радиусе `hoverRadius`; `Refresh` вызывает `ObserveEnemy(MainEnemy())`; `BuildEnemyText` берёт дистанцию из `DistanceToPlayer(_observedEnemy)`.
Блокер: —
