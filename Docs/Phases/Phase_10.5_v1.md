# Phase 10.5: Dev Tools (Инструменты разработчика)
- Status: in-progress
- Цель: изолированная от продакшн-кода dev-прослойка для тестирования — авто-проводка базовых систем (`GameServices`), Dev Menu с авто-регистрацией параметров (рефлексия по SO), Debug Overlay с числовым наблюдением статусов, отладочная визуализация пространственных механик и упорядоченная иерархия сцены. В релизном WebGL-билде dev-кода и dev-маркеров нет.

## Задачи (порядок = приоритет)
- T075: `GameServices` — статический service locator + саморегистрация PoolManager/StatusEffectSystem/ControlEffectSystem/OnHitDispatcher.
- T076: Миграция 5 on-hit-хендлеров на `GameServices` (убрать ручные поля-сервисы).
- T077: `DevParamRegistry` — рефлексия `float`/`int` полей SO, авто-регистрация параметров.
- T078: `DevBootstrap` + `DevMenuController` — программный Canvas, тогл F1/`~`, авто-ряды из реестра.
- T079: `DebugOverlayController` + dev-only аксессоры статусов (burn/freeze) в StatusEffectSystem/ControlEffectSystem.
- T080: `SceneOrganizer` (Editor) — группировка сцены по заголовкам.
- T081: `DevVisualizer` + отладочные маркеры (луч/сектор/окружность) + визуальные плейсхолдеры шасси/турели.

## Setup for smoke test (human)
1. T080: меню `Tools → Dev → Organize Scene Hierarchy` — сгруппировать сцену.
2. T081: добавить `ChassisVisual` (Cube) на Player и `TurretVisual` на тестовые турели.
3. Для проверки авто-регистрации: временно добавить в `EnemyData` поле `public float testParam;`
   (после проверки удалить).
4. Play Mode → F1 открывает Dev Menu; `~` тоже; Debug Overlay виден слева сверху.

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] У хендлеров (`Slow/Freeze/Burn/Explosion/Ground`) в Inspector нет полей
      `controlEffects`/`statusEffects`/`pool`/`onHit`; при этом Slow/Freeze/Burn/Explosion/Ground
      работают как в Phase 10 (регресс не сломан).
- [ ] F1/`~` открывает/закрывает Dev Menu; панель скрыта по умолчанию и не мешает игре.
- [ ] В Dev Menu автоматически видны: `EnemyData` (health/speed/damage/xpReward), `ChassisData`
      (health/speed), `TurretData → delivery` (damage/aps), `OnHitModuleData`
      (duration/tickDamage/tickRate/slowAmount…).
- [ ] Правка `EnemyData.health` меняет HP новых спавнов; значение живёт между рестартами раунда
      и сменой турели; после выхода из Play Mode ассет в Project не изменён.
- [ ] Новое поле в `EnemyData` появляется в Dev Menu без правок UI (авто-регистрация).
- [ ] Debug Overlay показывает числовые HP/скорость/модификаторы игрока и ближайшего врага +
      активные статусы (burn-остаток, freeze-стаки); новое публичное поле компонента появляется само.
- [ ] Hitscan оставляет линию от `muzzle` на `module.range`; Cone рисует сектор с `module.angle`/`module.range`;
      Explosion/Ground — окружности радиуса `module.radius`; маркеры отключаются тумблером.
- [ ] Корпус шасси (`ChassisVisual`) и `TurretVisual` видны в Play Mode.
- [ ] Иерархия сцены сгруппирована под `_Managers/_Player/_Combat/_SpawnPoints/_UI/_Dev`;
      Turret и тестовые турели остались дочерними Player.
- [ ] Релизный WebGL-билд (Development Build выкл): нет Dev Menu/Overlay/маркеров, F1 не работает;
      обычная игра не затронута.
