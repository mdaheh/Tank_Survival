# Phase Map (MVP)

Карта фаз MVP из `Docs/Architecture.md`. Фаза — вертикальный срез: работающая
сквозная функциональность, которую человек проверяет руками в конце фазы.
Порядок — порядок наращивания среза. Подробно расписаны Phase 1
(`Phase_1_mvp.md`), Phase 2 (`Phase_2_mvp.md`), Phase 3 (`Phase_3_mvp.md`);
фазы 4+ — строками ниже.

## Фазы

- **Phase 1 — Фундамент: данные и живой танк.** Игрок движется по вводу; статы
  (скорость, HP) читаются из `ChassisData` через `StatBlock` — данные не
  хардкодятся, есть composition root.
- **Phase 2 — Боевой цикл: враг и снаряд.** Турель Hammer стреляет снарядом по
  ближайшему врагу; враг Runner берётся из пула, идёт к игроку, умирает от
  урона через `IDamageable`, возвращается в пул.
- **Phase 3 — Волны и раунд.** `WaveSpawner` гонит волны по
  `WaveData`/`DifficultyData` из 4 точек; `RoundController` ведёт фазы,
  победу/поражение и рестарт без `LoadScene`; число объектов не растёт.
- **Phase 4 — Beam (hitscan instant).** Второй delivery-пайплайн: мгновенный
  raycast-урон через тот же `IDamageable`.
- **Phase 5 — On-hit модули.** Explosion (AoE) и Burn (DoT) как отдельный слой
  через `IHitHandler` поверх delivery; статусы тикают отдельной системой.
- **Phase 6 — Прогрессия и level-up.** XP за убийство, линейная кривая, пауза,
  3 карточки (Damage/FireRate/MoveSpeed/MaxHealth) пишут в `StatBlock` и
  реально усиливают следующий выстрел.
- **Phase 7 — HUD, завершение и выбор.** HUD (HP/XP/волна/убийства),
  `RoundEndUI` (победа/поражение), выбор шасси/турель/сложности в
  `SessionState`; полный smoke MVP по всем прогонам.
