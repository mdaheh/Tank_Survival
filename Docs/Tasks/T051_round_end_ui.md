# T051: RoundEndUI — победа/поражение + рестарт
- Status: [ ]
- Phase: 7
- Depends: T048
- Files:
  - Assets/Scripts/UI/RoundEndUI.cs

## Задача
Создать `RoundEndUI : MonoBehaviour` (namespace `TankSurvival.UI`; `using TankSurvival.Round;`, `using UnityEngine;`, `using UnityEngine.UI;`).

Поля: `public RoundController roundController;`, `public GameObject panel;`, `public Text resultText;`, `public Button restartButton;`.

- `private void Awake()`: `roundController.OnPhaseChanged += HandlePhaseChanged;`, `restartButton.onClick.AddListener(Restart);`, `panel.SetActive(false);`.
- `private void HandlePhaseChanged(RoundController.RoundPhase phase)`: если `phase == RoundController.RoundPhase.Win` → `resultText.text = "Победа"; Show();`; иначе если `phase == RoundController.RoundPhase.Lose` → `resultText.text = "Поражение"; Show();` (на `Playing`/`LevelUp` ничего не делать).
- `private void Show()`: `Time.timeScale = 0f; panel.SetActive(true);`.
- `private void Restart()`: `Time.timeScale = 1f; panel.SetActive(false); roundController.Restart();`.

## Ограничения
- Без `SceneManager.LoadScene` — рестарт через `roundController.Restart()`.
- `Time.timeScale` меняется только в `Show` (0) и `Restart` (1); `Update` нет.
- Тексты «Победа»/«Поражение» — русские плейсхолдеры (локализация — позже).

## Unity (человеку)
1. В сцене на Canvas: панель `RoundEndPanel` (неактивна) → Text `ResultText` + Button `RestartButton` (с дочерним Text «Рестарт»).
2. Создать GameObject `RoundEndUI`, добавить компонент `RoundEndUI`.
3. Заполнить: `roundController` → `RoundController`, `panel` → `RoundEndPanel`, `resultText` → `ResultText`, `restartButton` → `RestartButton`.

## Проверка человеком
- Play Mode: зачищены все волны → активен `RoundEndPanel`, текст «Победа», `Time.timeScale = 0`.
- Смерть игрока (HP = 0) → активен `RoundEndPanel`, текст «Поражение», `Time.timeScale = 0`.
- Клик «Рестарт» → панель скрыта, `Time.timeScale = 1`, новый раунд: `currentWave = 1`, `kills = 0`, `xp = 0`, `level = 1`, статы в `StatBlock` вернулись к базовым.

## Отчёт Исполнителя
Готово:
Блокер:
