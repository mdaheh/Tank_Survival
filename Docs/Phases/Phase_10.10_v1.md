# Phase 10.10: Аудит P0 — критичные guard'ы и сброс статики
- Status: in-progress
- Цель: закрыть P0-находки аудита Phase 10.8: guard от двойного возврата в `PoolManager.Release`, явная точка сброса статического реестра `GameServices` между Play Mode-сессиями, guard после смерти в `PlayerHealth.TakeDamage`.

## Задачи (порядок = приоритет)
- T101: `PoolManager.Release` — guard от повторного возврата уже неактивного объекта в пул.
- T102: `GameServices` — `Clear()` + editor-сброс реестра на выходе из Play Mode.
- T103: `PlayerHealth.TakeDamage` — guard после смерти (не пропускать повторный урон и повторный `OnDeath`).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Play Mode → Start Round → несколько волн: пул врагов стабилен, нет «призрачных»/задвоенных объектов, в Console нет «already-pooled».
- [ ] Повторный вход в Play Mode (при выключенном Reload Domain) не даёт stale-сервисов: бой и Dev Menu работают с первого запуска, в Console нет NRE / MissingReferenceException.
- [ ] Смерть игрока (одновременные удары нескольких врагов): экран Lose появляется один раз, HP не уходит ниже 0 повторными ударами.
