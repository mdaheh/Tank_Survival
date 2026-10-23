# Phase 10.8: Dev Menu — переосмысление + каркас паузы
- Status: in-progress
- Цель: превратить Dev Menu из оверлея (F1/`~`) в экран настроек перед запуском раунда (показ при старте, секции, ручной ввод, корректное форматирование, рабочий скролл, кнопки Start/Reset) и добавить минимальный каркас паузы (Esc → Продолжить/Выйти); кнопка RoundEnd ведёт в Dev Menu.

## Задачи (порядок = приоритет)
- T087: `RoundController` — `isRoundActive` + `AbortRound()` (досрочное завершение без Win/Lose).
- T088: Dev Menu как экран перед раундом — показ при старте, убрать F1/`~`, кнопка «Start Round»; `GameBootstrap` не автостартует раунд в dev.
- T089: Dev Menu — секции/категории (Chassis/Turret/Delivery/OnHit/Difficulty/Enemy).
- T090: Dev Menu — ручной ввод точных чисел (InputField) + корректное форматирование int/float.
- T091: Dev Menu — исправление скролла списка.
- T092: `PauseMenuController` — минимальный каркас паузы (Esc / Продолжить / Выйти / guard).
- T093: `RoundEndUI` — кнопка ведёт в Dev Menu (событие `OnContinue`).
- T094: Dev Menu — связка с паузой и RoundEnd (показ Dev Menu после «Выйти» и после завершения раунда).
- T095: Dev Menu — кнопка «Reset» (сброс параметров + обновление UI).

## Setup for smoke test (human)
1. T092: на игровом Canvas создать `PausePanel` (заголовок «ПАУЗА», кнопки `ResumeButton`/`ExitButton`) и GameObject `PauseMenuController`; заполнить поля: `Round Controller` → `RoundController`, `Panel` → `PausePanel`, `Resume Button` → `ResumeButton`, `Exit Button` → `ExitButton`.
2. Убедиться, что в сцене присутствуют и заполнены `RoundController` (sessionState/waveSpawner/playerHealth/statBlock/progression) и `SessionState` (selectedChassis/selectedTurret/selectedDifficulty) — как в прошлых фазах.

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Play Mode (Editor/DEBUG) → сразу Dev Menu; раунд не начат (`RoundController` → `Is Round Active` = false, врагов нет).
- [ ] Dev Menu: параметры сгруппированы по секциям (Chassis/Delivery/OnHit/Difficulty/Enemy); у каждой строки — поле ручного ввода.
- [ ] int-поля целые, float — с точкой и без лишних нулей (например `1.5`, а не `1,500`).
- [ ] Ручной ввод точного числа сохраняется (например `pelletCount` = 25), слайдер ограничен, но значение точное.
- [ ] Скролл списка работает колесом мыши над любой частью списка (не только над слайдером).
- [ ] Кнопка «Reset» возвращает изменённые параметры к исходным.
- [ ] «Start Round» запускает раунд с текущими настройками; F1/`~` не открывают меню.
- [ ] Esc во время раунда → «ПАУЗА» (`Time.timeScale = 0`); «Продолжить» → раунд идёт; «Выйти» → раунд завершён без RoundEndUI, показан Dev Menu.
- [ ] Esc до старта и после победы/поражения не открывает паузу.
- [ ] После победы/поражения кнопка RoundEnd ведёт в Dev Menu (не автозапуск нового раунда).
