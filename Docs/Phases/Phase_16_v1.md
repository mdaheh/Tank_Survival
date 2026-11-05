# Phase 16: Стрельба v2 — модификаторы Pierce, Homing, Beam Split, Laser Focus
- Цель: добавить четыре on-hit модификатора (`Pierce`, `Homing`, `Beam Split`, `Laser Focus`) как lifecycle-модули стрельбы и расширить сборку `BuildSnapshot` (`ShotFactory`) так, чтобы модули попадали в нужные стадийные массивы (`spawn`/`flight`/`hit`/`postHit`), а не только в `hitModules`; per-shot урон перенести в `ShotContext` для поддержки Pierce-falloff.

## Unity (человеку)
1. Создать 4 ассета (Project → Create → Tank Survival → OnHit Module Data) в `Assets/Data/OnHit/`:
   - `Pierce.asset`: `kind = Pierce`, `priority = 2`, `maxPierceCount = 3`, `damageFalloff = 0.8`, `maxDepth = 5`.
   - `Homing.asset`: `kind = Homing`, `priority = 1`, `homingStrength = 3`, `homingRadius = 6`, `maxDepth = 5`.
   - `BeamSplit.asset`: `kind = BeamSplit`, `priority = 6`, `splitCount = 3`, `splitAngle = 45`, `maxDepth = 5`.
   - `LaserFocus.asset`: `kind = LaserFocus`, `priority = 9`, `damageGrowth = 0.1`, `maxStacks = 5`, `stackResetOnSwitch = true`, `maxDepth = 5`.
2. Привязать модули к турелям в `TurretData.onHitModules` (затем проверить в Play Mode):
   - снарядная турель (Hammer) → `Pierce.asset` — проверка пробития;
   - снарядная турель (Hammer) → `Homing.asset` — проверка наведения;
   - hitscan-турель → `BeamSplit.asset` — проверка разделения луча;
   - непрерывная hitscan-турель (Laser) → `LaserFocus.asset` — проверка роста урона.

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] Pierce: снаряд пробивает несколько врагов подряд (счётчик), урон каждой следующей цели падает (× `damageFalloff`), после исчерпания пробитий снаряд уничтожается на попадании.
- [x] Homing: снаряд доворачивает к ближайшему врагу в `homingRadius` (в Debug виден голубой круг `DevVisualizer.ShowCircle`); без цели летит прямо.
- [x] Beam Split: попадание луча порождает `splitCount` лучей-детей из точки попадания, распределённых по `splitAngle`; дети наносят урон, наследуют билд и могут разделяться дальше до `maxDepth`.
- [x] Laser Focus: урон непрерывного луча растёт по стакам на одной цели до `maxStacks`; при смене цели стаки и бонус урона сбрасываются; на мгновенном hitscan модуль не срабатывает.
- [x] Объекты не накапливаются: пул `ShotContext`/`ShotView`/VFX стабилен.

## Примечания
- Модули реализованы по контрактам `Docs/Shooting.md`: Pierce — `IOnSpawn` + `IOnPostHit` (счётчик и falloff урона в `ctx`, `keepAlive` для продолжения полёта); Homing — `IOnFlight`; Beam Split — `IOnHit` + `spawnQueue` (полное наследование `ctx.buildSnapshot`); Laser Focus — `IOnHit` (стаки в `ctx`).
- `ShotFactory.PopulateModules` теперь раскладывает модули по стадийным массивам, `DamageModule` остаётся первым в `hitModules`. Порядок on-hit модулей — по убыванию `priority` (как раньше).
- Per-shot урон вынесен в `ShotContext.damage` (инициализируется из `buildSnapshot.damage` в `ShotSystem.Fire`); `DamageModule` читает `ctx.damage` — это нужно для Pierce-falloff.
- Laser Focus добавляет бонусный урон отдельным вызовом (`damage × damageGrowth × stacks`), не модифицируя базовый урон `DamageModule` — так сброс стаков при смене цели не конфликтует с порядком `hit`-модулей.
- Homing: скорость доворота = `homingStrength × 60°/с` (константа в модуле); `homingRadius`/`homingStrength` — числовые поля SO (Legend, шкала «сила/наведение»: medium = 3). В smoke-ассете радиус 6 взят для наглядности, GDD medium = 3.
- Требования совместимости модулей (Pierce/Homing → projectile, Beam Split → hitscan, Laser Focus → hitscan + continuous) на уровне тегов кодом пока не проверяются; Beam Split рассчитан на мгновенный hitscan (на continuous-лазере будет спавнить детей каждый тик).
- Новые `.asset`-файлы создаёт человек в Unity (YAML не редактируем агентом).
