# T104: Возврат в меню — полный сброс состояния + гейт движения
- Status: [x]
- Phase: 10.10
- Files:
  - Assets/Scripts/Round/RoundController.cs
  - Assets/Scripts/UI/RoundEndUI.cs
  - Assets/Scripts/UI/PauseMenuController.cs
  - Assets/Scripts/Player/PlayerController.cs

## Задача
«Возврат в меню» (кнопка на экране RoundEnd и «Выйти» из паузы) должен полностью возвращать игру в стартовое состояние: сбрасывать статы/прогрессию/здоровье и останавливать управление игроком вне активного раунда.

1. `Assets/Scripts/Round/RoundController.cs` — заменить метод `AbortRound()` на `ResetToMenu()` с телом:
```csharp
public void ResetToMenu()
{
    isRoundActive = false;
    currentWave = 0;
    kills = 0;
    waveSpawner.ClearActiveEnemies();
    statBlock.Init(sessionState.SelectedChassis, sessionState.SelectedTurret);
    progression.Reset();
    playerHealth.Init();
    OnWaveChanged?.Invoke();
    OnKillsChanged?.Invoke();
}
```

2. `Assets/Scripts/UI/RoundEndUI.cs` — в методе `GoToMenu()`, после `panel.SetActive(false);`, добавить:
```csharp
roundController.ResetToMenu();
```

3. `Assets/Scripts/UI/PauseMenuController.cs` — в методе `ExitToMenu()` заменить `roundController.AbortRound();` на `roundController.ResetToMenu();`.

4. `Assets/Scripts/Player/PlayerController.cs` — гейт движения:
   - добавить `using TankSurvival.Round;`;
   - добавить поле `[SerializeField] private RoundController roundController;`;
   - в начало тела `Update()` добавить:
```csharp
if (roundController == null
    || !roundController.isRoundActive
    || roundController.phase != RoundController.RoundPhase.Playing)
{
    return;
}
```

## Ограничения
- `ResetToMenu()` не меняет `phase` и не вызывает `SetPhase`/`OnPhaseChanged` — иначе `RoundEndUI.HandlePhaseChanged` снова покажет экран победы/поражения.
- Условие гейта движения совпадает с `PauseMenuController.CanPause()` (`isRoundActive && phase == Playing`): игрок управляется только во время идущего раунда.
- `statBlock.Init(...)`, `progression.Reset()`, `playerHealth.Init()`, `waveSpawner.ClearActiveEnemies()` — уже существующие методы (используются в `StartRound()`/`AbortRound()`). Новых using в `RoundController` не требуется (те же типы уже используются в `StartRound()`).
- `ResetToMenu()` идемпотентен: можно вызывать после смерти, когда враги уже очищены.

## Unity (человеку)
- На объекте Player (компонент `PlayerController`) заполнить новое поле `Round Controller`: перетащить объект сцена с компонентом `RoundController`.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu → Start Round → довести до смерти → экран «Поражение» → клик по кнопке → открывается Dev Menu; игрок НЕ двигается по WASD; HUD показывает волну «0 / N» и полное HP; Debug Overlay (блок «Игрок») показывает `StatBlock` с дефолтными значениями (не со статами на момент смерти).
- Тот же результат при выходе из паузы («Выйти»): состояние сброшено, игрок не управляется.
- После этого «Start Round» стартует новый раунд с дефолтными статами (волна 1, HP полное).

## Отчёт Исполнителя
Готово: Assets/Scripts/Round/RoundController.cs: `AbortRound()` заменён на `ResetToMenu()` (сброс флагов волны/килов, очистка врагов, `statBlock.Init`, `progression.Reset`, `playerHealth.Init`, `OnWaveChanged`/`OnKillsChanged`); Assets/Scripts/UI/RoundEndUI.cs: в `GoToMenu()` после `panel.SetActive(false)` добавлен `roundController.ResetToMenu()`; Assets/Scripts/UI/PauseMenuController.cs: в `ExitToMenu()` вызов заменён на `roundController.ResetToMenu()`; Assets/Scripts/Player/PlayerController.cs: добавлен `using TankSurvival.Round`, поле `roundController` и гейт в начале `Update()`.
Блокер: —
