# Phase 7: HUD, завершение и выбор
- Status: accepted
- Цель: замкнуть MVP пользовательским слоем — HUD (HP/XP/волна/убийства), экран победы/поражения с рестартом без `LoadScene`, единый выбор шасси/турели/сложности в `SessionState` — и прогнать полный smoke по трём прогонам (A/B/C).

## Задачи (порядок = приоритет)
- T046: Враг Tank (data-only) — `Tank.asset` + `Tank.prefab` + состав в `WaveData`.
- T047: `SessionState` — `selectedDifficulty` (+ `SelectedDifficulty`).
- T048: `RoundController`/`WaveSpawner` — сложность из `SessionState`, счётчик kills, события фазы/волны/убийств.
- T049: `PlayerHealth`/`ProgressionSystem` — события HP/XP для HUD.
- T050: `HUDController` — HP/XP-бар/волна/убийства.
- T051: `RoundEndUI` — победа/поражение + рестарт.

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] HUD показывает HP (текущее/макс), прогресс XP, уровень, волну (текущая/всего), убийства.
- [x] Убийство врага инкрементит счётчик убийств; урон игроку обновляет HP-бар; XP-бар растёт от убийств.
- [x] Зачистка волны → номер волны растёт; после последней волны — `RoundEndUI` показывает «Победа».
- [x] HP игрока = 0 → `RoundEndUI` показывает «Поражение».
- [x] Кнопка рестарта в `RoundEndUI` запускает новый раунд без перезагрузки сцены (состояние сброшено, `Time.timeScale = 1`).
- [x] Смена `selectedChassis`/`selectedTurret`/`selectedDifficulty` в `SessionState` меняет статы/турель/число волн следующего раунда.
- [x] Полный smoke по прогонам A (Standart+Hammer), B (Standart+Beam), C (Fat+любая турель): оба врага (Runner+Tank) в раунде, число объектов в иерархии не растёт, консоль чистая.
