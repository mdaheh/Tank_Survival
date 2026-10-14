# T042: LevelUpUI — пауза + 3 карточки
- Status: [x]
- Phase: 6
- Depends: T041
- Files:
  - Assets/Scripts/UI/LevelUpUI.cs

## Задача
Создать `LevelUpUI : MonoBehaviour` (namespace `TankSurvival.UI`, `using System.Collections.Generic;`, `using TankSurvival.Data;`, `using TankSurvival.Progression;`, `using UnityEngine;`, `using UnityEngine.UI;`).

Поля: `public ProgressionSystem progression;`, `public GameObject panel;`, `public Button[] cardButtons;`, `public Text[] cardLabels;`.

- `private void Awake()`: `progression.OnLevelUp += Show;` и `panel.SetActive(false);`.
- `private void Show()`: `List<UpgradeData> upgrades = progression.RollUpgrades(3);` затем для каждого `i < cardButtons.Length`:
  если `i < upgrades.Count` — включить кнопку, выставить `cardLabels[i].text = upgrades[i].stat.ToString() + " +" + upgrades[i].value;`,
  снять старые слушатели (`onClick.RemoveAllListeners()`), добавить `onClick.AddListener(() => Choose(upgrades[i]))` (захват в локальную переменную);
  иначе `cardButtons[i].gameObject.SetActive(false);`.
  В конце: `Time.timeScale = 0f; panel.SetActive(true);`.
- `private void Choose(UpgradeData upgrade)`: если `progression.ChooseUpgrade(upgrade)` → `panel.SetActive(false); Time.timeScale = 1f;`.

## Ограничения
- Текст карточки — MVP-плейсхолдер (`stat + " +" + value`); локализация — позже.
- `Time.timeScale` меняется только в `Show` (0) и `Choose` (1); `Update` у класса нет.

## Unity (человеку)
1. В `Assets/UI/` (или в сцене) собрать UI: Canvas → панель `LevelUpPanel` (неактивна) → 3 кнопки `Card_1..3`, в каждой дочерний `Text` (Label).
2. Создать GameObject `LevelUpUI`, добавить компонент `LevelUpUI`.
3. Заполнить: `progression` → `ProgressionSystem`, `panel` → `LevelUpPanel`, `cardButtons` → 3 кнопки, `cardLabels` → 3 текста.

## Проверка человеком
- По достижении level-up игра ставится на паузу, `LevelUpPanel` активен, видны до 3 карточек.
- Клик по карточке → панель скрыта, `Time.timeScale = 1`, стат в `StatBlock` увеличился.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/UI/LevelUpUI.cs — создан LevelUpUI: подписка на OnLevelUp в Awake, Show (RollUpgrades(3), до 3 карточек, пауза Time.timeScale = 0), Choose (снятие паузы после успешного ChooseUpgrade).
Блокер: —
