# T007: GameBootstrap — composition root
- Status: [x]
- Phase: 1
- Priority: Critical
- Depends: T002, T004, T006
- Files:
  - Assets/Scripts/Core/GameBootstrap.cs
  - Assets/Scripts/Core/SessionState.cs
  - Assets/Scripts/Core/StatBlock.cs
  - Assets/Scripts/Player/PlayerHealth.cs

## Задача
Создать класс `GameBootstrap : MonoBehaviour` с сериализованными полями `SessionState sessionState`, `StatBlock statBlock`, `PlayerHealth playerHealth`. В `Start()` вызвать строго по порядку: `statBlock.Init(sessionState.SelectedChassis);` затем `playerHealth.Init();`.

## Ограничения
- Порядок инициализации строго: StatBlock → PlayerHealth.
- Нет `SceneManager.LoadScene`, нет `Instantiate`/`Destroy`.

## Unity (человеку)
- Собрать сцену:
  - GameObject "Bootstrap": компоненты `SessionState` + `GameBootstrap`.
  - GameObject "Player": компоненты `StatBlock` + `PlayerController` + `PlayerHealth`.
- Привязать ссылки: `GameBootstrap.sessionState` / `statBlock` / `playerHealth`;
  `PlayerController.statBlock` и `PlayerController.moveAction`; `PlayerHealth.statBlock`.
- `SessionState.SelectedChassis` = `Assets/Settings/Chassis/Standart.asset`.

## Проверка человеком
- Play Mode: танк движется по WASD со скоростью Standart (5.0); `PlayerHealth.currentHealth` = `StatBlock.maxHealth` = 100.
- Смена `SelectedChassis` на `Fat.asset` (без правки кода) → скорость 2.5, `maxHealth` 200.

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/GameBootstrap.cs — создан класс GameBootstrap : MonoBehaviour с сериализованными полями sessionState, statBlock, playerHealth; в Start() строго по порядку: statBlock.Init(sessionState.SelectedChassis), затем playerHealth.Init(); Assets/Scripts/Core/SessionState.cs, Assets/Scripts/Core/StatBlock.cs, Assets/Scripts/Player/PlayerHealth.cs — без изменений, использованы как есть.
Блокер: —
