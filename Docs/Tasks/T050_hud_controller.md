# T050: HUDController — HP/XP/волна/убийства
- Status: [ ]
- Phase: 7
- Depends: T048, T049
- Files:
  - Assets/Scripts/UI/HUDController.cs

## Задача
Создать `HUDController : MonoBehaviour` (namespace `TankSurvival.UI`; `using TankSurvival.Player;`, `using TankSurvival.Progression;`, `using TankSurvival.Round;`, `using UnityEngine;`, `using UnityEngine.UI;`). HUD только читает события, не пишет состояние.

Поля: `public PlayerHealth playerHealth;`, `public ProgressionSystem progression;`, `public RoundController roundController;`, `public Image hpFill;`, `public Text hpText;`, `public Image xpFill;`, `public Text xpText;`, `public Text levelText;`, `public Text waveText;`, `public Text killsText;`.

- `private void Awake()`: подписаться `playerHealth.OnHealthChanged += UpdateHealth;`, `progression.OnProgressChanged += UpdateProgression;`, `roundController.OnWaveChanged += UpdateWave;`, `roundController.OnKillsChanged += UpdateKills;` (начальное состояние приходит из событий `StartRound`).
- `private void UpdateHealth(float current, float max)`: `hpFill.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;` и `hpText.text = Mathf.CeilToInt(Mathf.Max(0f, current)) + " / " + Mathf.CeilToInt(max);`.
- `private void UpdateProgression()`: `float need = progression.XpNeeded;` → `xpFill.fillAmount = need > 0f ? Mathf.Clamp01(progression.xp / need) : 0f;`, `xpText.text = Mathf.CeilToInt(progression.xp) + " / " + Mathf.CeilToInt(need);`, `levelText.text = progression.level.ToString();`.
- `private void UpdateWave()`: `waveText.text = roundController.currentWave + " / " + roundController.TotalWaves;`.
- `private void UpdateKills()`: `killsText.text = roundController.kills.ToString();`.

## Ограничения
- Только чтение; никаких записей в подсистемы.
- Нет `Update()`; без аллокаций; тексты — русские плейсхолдеры без локализации (как `LevelUpUI`).
- Не использовать `Slider` — бары через `Image` с `Image Type = Filled`.

## Unity (человеку)
1. В сцене на Canvas создать GameObject `HUD`, добавить компонент `HUDController`.
2. Собрать элементы UI (дочерние `HUD`):
   - Image `HpFill` (Image Type = Filled, Fill Method = Horizontal) + Text `HpText` — левый верхний угол;
   - Image `XpFill` (Filled, Horizontal) + Text `XpText` + Text `LevelText` — внизу по центру;
   - Text `WaveText` — правый верхний;
   - Text `KillsText` — под `WaveText`.
3. Заполнить `HUDController`: `playerHealth` → `PlayerHealth` (на `Player`), `progression` → `ProgressionSystem`, `roundController` → `RoundController`; `hpFill`/`hpText`/`xpFill`/`xpText`/`levelText`/`waveText`/`killsText` → соответствующие объекты.

## Проверка человеком
- Play Mode: HUD показывает «текущее/макс» HP, прогресс XP (`xp / need`), уровень, волну (`текущая/всего`), убийства.
- Убийство врага → `KillsText` растёт; урон игроку → HP-бар и `HpText` обновляются; убийства → XP-бар растёт, `LevelText` растёт после level-up.
- Зачистка волны → `WaveText` растёт.

## Отчёт Исполнителя
Готово:
Блокер:
