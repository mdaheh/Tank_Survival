# Phase 6: Прогрессия и level-up
- Status: in-progress
- Цель: XP за убийство по линейной кривой (`5 + N × 3`), level-up с паузой и выбором одной из 3 карточек (Damage/FireRate/MoveSpeed/MaxHealth), которые аддитивно пишутся в `StatBlock` и реально усиливают следующий выстрел/движение/HP.

## Задачи (порядок = приоритет)
- T037: `DifficultyData.xpMultiplier` — множитель XP сложности.
- T038: `UpgradeData` — SO апгрейда (stat/value/maxStacks).
- T039: `StatBlock` — источник статов + `ApplyUpgrade`; `SessionState.SelectedTurret`; sync MaxHealth→HP.
- T040: Combat читает damage/fireRate из `StatBlock`.
- T041: `ProgressionSystem` — XP, кривая, level-up, пул карточек, стеки.
- T042: `LevelUpUI` — пауза + 3 карточки + выбор.
- T043: XP за убийство в `WaveSpawner`.
- T044: `RoundController` — фаза LevelUp + сброс раунда (StatBlock/Progression).
- T045: `GameBootstrap` — финальная чистка (убрать дублирующий init).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок.
- [ ] Убийство врага увеличивает `ProgressionSystem.xp` (видно в инспекторе).
- [ ] По достижении порога `5+N×3` — игра встаёт на паузу (`Time.timeScale = 0`), видна панель с 3 карточками.
- [ ] Выбор карточки Damage → следующий выстрел сносит больше HP (враг умирает быстрее).
- [ ] Выбор карточки FireRate → турель стреляет чаще (кулдаун меньше).
- [ ] Выбор карточки MoveSpeed → игрок движется быстрее.
- [ ] Выбор карточки MaxHealth → `StatBlock.maxHealth` и `PlayerHealth.currentHealth` растут на +20.
- [ ] Рестарт раунда (Context Menu → Restart Round) обнуляет XP/уровень и возвращает статы к базовым.
- [ ] Карточки в одном предложении не повторяются; в консоли нет ошибок.
