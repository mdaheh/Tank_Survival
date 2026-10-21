# Phase Map

Карта фаз из `Docs/Architecture.md`. Фаза — вертикальный срез: работающая
сквозная функциональность, которую человек проверяет руками в конце фазы.
Порядок — порядок наращивания среза.

**Именование файлов фаз:** фазы MVP — `Phase_N_mvp.md`,
фазы v1.0 — `Phase_N_v1.md`.

## MVP — закрыто (accepted)

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

Подробно: `Phase_1_mvp.md` … `Phase_7_mvp.md`.

## v1.0 — в работе

### Контент / «играть дольше и стабильнее»

- **Phase 8 — Враги v1.0: Brute, Shooter, Boss.** [accepted]
- **Phase 9 — Delivery полностью: Cone + continuous Hitscan.** [accepted]
- **Phase 10 — On-hit контроль: Freeze, Slow, Knockback, Ground.** [accepted]
- **Phase 10.5 — Dev Tools: DI (`GameServices`), Dev Menu, Debug Overlay, dev-маркеры.** [in-progress]
- **Phase 11 — On-hit модификаторы: Pierce, Homing, Beam Split, Laser Focus.** [planned]
- **Phase 12 — Специализации турелей (8) и три ветки.** [planned]
- **Phase 13 — Шасси v1.0: Fancy, Veteran + каркас способностей + Boost.** [planned]
- **Phase 14 — Способности: Nova, Phase, Shield.** [planned]
- **Phase 15 — Полный level-up: пул апгрейдов (51 SO), кривая 30, cap.** [planned]
- **Phase 16 — Полная система волн: батч 20, босс на 10-й.** [planned]
- **Phase 17 — Wave reward: модульные апгрейды (каждая 5-я волна).** [planned]

### Осмысленность / мета и UI

- **Phase 18 — Мета-прогрессия: счётчики, LocalStorage, разблокировки.** [planned]
- **Phase 19 — Все 8 сложностей и последовательное открытие.** [planned]
- **Phase 20 — UI-экраны: Main Menu, Hangar, Difficulty, Pause + локализация RU/EN.** [planned]
- **Phase 21 — Round End полный: итоги и уведомления об открытиях.** [planned]

### Запуск

- **Phase 22 — Монетизация: SDK + rewarded-сценарии.** [planned]
- **Phase 23 — Аудио: фоновая музыка + SFX.** [planned]
- **Phase 24 — WebGL-сборка и оптимизация (+ финальный смоук v1.0).** [planned]

## Критерии готовности v1.0

Чек-лист ручного прогона после Phase 24:

- [ ] Полный проход раунда на всех 8 сложностях (победа по правилам GDD).
- [ ] Все 4 шасси + 4 способности работают (Boost/Nova/Phase/Shield).
- [ ] Все 3 ветки + 8 специализаций стреляют по GDD.
- [ ] Все 10 on-hit-модулей работают; конфликты (burn/freeze), priority,
  max_depth соблюдены.
- [ ] Все 5 врагов спавнятся и ведут себя по GDD (Shooter стреляет, Boss на
  10-й волне).
- [ ] Level-up предлагает корректный пул по билду; стеки additive →
  multiplicative; cap на 30-м уровне.
- [ ] Wave reward каждую 5-ю волну добавляет выбранный модуль к турели.
- [ ] Мета-прогрессия сохраняется в LocalStorage; разблокировки
  турелей/шасси/сложностей применяются.
- [ ] Полная навигация по 6 экранам; пауза с подтверждением; локализация
  RU/EN.
- [ ] Rewarded-сценарии работают через SDK (continue ×1, ×2 результата).
- [ ] Аудио на ключевых событиях (музыка + SFX).
- [ ] WebGL-сборка стабильна: 60 FPS (мин. 30), ноль аллокаций/Instantiate в
  цикле, деградация при превышении лимитов.
