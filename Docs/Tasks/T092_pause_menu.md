# T092: PauseMenuController — минимальный каркас паузы
- Status: [x]
- Phase: 10.8
- Depends: T087
- Files:
  - Assets/Scripts/UI/PauseMenuController.cs

## Задача
Создать `Assets/Scripts/UI/PauseMenuController.cs` — класс `PauseMenuController : MonoBehaviour` (namespace `TankSurvival.UI`). Минимальная пауза по GDD: Esc во время раунда ставит `Time.timeScale = 0` и показывает панель с кнопками «Продолжить» и «Выйти».

Поля (public, заполняются в инспекторе):
```csharp
public RoundController roundController;
public GameObject panel;
public Button resumeButton;
public Button exitButton;
```
Событие: `public event System.Action OnExitToMenu;` (на него позже подпишется Dev Menu).

Методы:
```csharp
private void Awake()
{
    resumeButton.onClick.AddListener(Resume);
    exitButton.onClick.AddListener(ExitToMenu);
    panel.SetActive(false);
}

private void Update()
{
    Keyboard keyboard = Keyboard.current;
    if (keyboard == null) return;
    if (!keyboard.escapeKey.wasPressedThisFrame) return;

    if (panel.activeSelf) { Resume(); return; }
    if (CanPause()) Show();
}

private bool CanPause()
{
    return roundController.isRoundActive
        && roundController.phase == RoundController.RoundPhase.Playing;
}

private void Show()
{
    Time.timeScale = 0f;
    panel.SetActive(true);
}

private void Resume()
{
    Time.timeScale = 1f;
    panel.SetActive(false);
}

private void ExitToMenu()
{
    Time.timeScale = 1f;
    panel.SetActive(false);
    roundController.AbortRound();
    OnExitToMenu?.Invoke();
}
```
`using`: `TankSurvival.Round`, `UnityEngine`, `UnityEngine.InputSystem`, `UnityEngine.UI`.

## Ограничения
- Guard: пауза открывается только при `isRoundActive && phase == Playing` (не при LevelUp/Win/Lose и не до старта раунда).
- «Выйти» завершает раунд досрочно без победы/поражения: `AbortRound()` не меняет фазу на Win/Lose, `RoundEndUI` НЕ показывается.
- `Time.timeScale` меняется только в `Show` (0) и `Resume`/`ExitToMenu` (1); в `Update` нет его изменения.

## Unity (человеку)
1. На игровом Canvas создать панель `PausePanel` (изначально неактивна): Text `PauseTitle` («ПАУЗА»), Button `ResumeButton` (дочерний Text «Продолжить»), Button `ExitButton` (дочерний Text «Выйти»).
2. Создать GameObject `PauseMenuController`, Add Component → `PauseMenuController`.
3. Заполнить: `Round Controller` → объект `RoundController`; `Panel` → `PausePanel`; `Resume Button` → `ResumeButton`; `Exit Button` → `ExitButton`.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → «Start Round» → во время раунда Esc → панель «ПАУЗА» активна, `Time.timeScale = 0` (игрок и враги замерли).
- «Продолжить» → панель скрыта, `Time.timeScale = 1`, раунд продолжается.
- «Выйти» → панель скрыта, `Time.timeScale = 1`, враги убраны со сцены, `RoundEndUI` не появился, `RoundController` → `Is Round Active` = false. (Dev Menu покажется в T094.)
- Esc до старта раунда (когда на экране Dev Menu) и после победы/поражения — пауза НЕ появляется.

## Отчёт Исполнителя
Готово: Assets/Scripts/UI/PauseMenuController.cs: создан класс `PauseMenuController : MonoBehaviour` (namespace `TankSurvival.UI`) с полями `roundController`/`panel`/`resumeButton`/`exitButton`, событием `OnExitToMenu` и методами `Awake`/`Update`/`CanPause`/`Show`/`Resume`/`ExitToMenu` точно по ТЗ.
Блокер: —
