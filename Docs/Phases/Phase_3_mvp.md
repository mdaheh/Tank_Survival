# Phase 3: Волны и раунд
- Status: in-progress
- Цель: `WaveSpawner` гонит 3 волны по `WaveData`/`DifficultyData` из 4 точек;
  `RoundController` ведёт фазы Playing/Win/Lose и рестарт без `LoadScene`; число
  объектов в иерархии между волнами не растёт.

## Задачи (порядок = приоритет)
- T019: `WaveData` + `DifficultyData` — SO волн и сложности (состав, интервал, число волн, множители).
- T020: Множители сложности — `EnemyHealth`/`EnemyController` применяют health/speed-множитель.
- T021: `WaveSpawner` + `SpawnPoint` — батч-спавн волн по данным из точек, счётчик живых, событие конца волны.
- T022: `RoundController` — фазы Playing/Win/Lose, победа/поражение, рестарт без `LoadScene`.
- T023: `GameBootstrap` — пересборка: убрать временный спавн, prewarm волн, запуск/рестарт раунда.

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Раунд стартует: 3 волны врагов спавнятся по `WaveData` из точек `SpawnPoint_1..4`.
- [ ] После зачистки 3-й волны `RoundController.phase = Win`.
- [ ] При HP игрока = 0 → `RoundController.phase = Lose`, враги возвращаются в пул.
- [ ] `Context Menu → Restart Round` стартует раунд заново без перезагрузки сцены.
- [ ] Между волнами число активных объектов в иерархии не растёт (нет `Instantiate`/`Destroy` в игре).
- [ ] В консоли нет ошибок.
