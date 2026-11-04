# Phase 11: On-hit модификаторы — Pierce, Homing, Beam Split, Laser Focus
- Status: in-progress
- Цель: добавить 4 оставшихся on-hit-модуля как «модификаторы доставки»: Pierce (пробитие) и Homing (самонаведение) работают на уровне снаряда, Beam Split (разлёт лучей) и Laser Focus (накопление урона) — на уровне hitscan. Модули резолвятся delivery-хендлерами через `OnHitDispatcher.GetModule`, интерфейс `IHitHandler` не меняется.

## Задачи (порядок = приоритет)
- T106: `OnHitModuleData` — 4 новых `OnHitKind` и 9 полей (Pierce/Homing/BeamSplit/LaserFocus).
- T107: `OnHitDispatcher.GetModule(kind)` — поиск модуля по kind для delivery-хендлеров.
- T108: Pierce — снаряд пробивает врагов с falloff, отслеживает уже поражённых.
- T109: Homing — снаряд самонаводится на ближайшего врага в радиусе.
- T110: Beam Split — hitscan-луч разлетается на `splitCount` лучей (depth-guard).
- T111: Laser Focus — continuous hitscan накапливает урон, сброс при смене цели.

## Setup for smoke test (human)
1. Создать 4 OnHitModuleData-ассета в `Assets/Data/OnHit/` (Create → Tank Survival → OnHit Module Data):
   - `Pierce.asset`: kind=Pierce, maxPierceCount=3, damageFalloff=0.8, priority=2.
   - `Homing.asset`: kind=Homing, homingStrength=3, homingRadius=3, priority=1.
   - `BeamSplit.asset`: kind=BeamSplit, splitCount=3, splitAngle=45, priority=6.
   - `LaserFocus.asset`: kind=LaserFocus, damageGrowth=0.1, maxStacks=5, stackResetOnSwitch=1, priority=9.
2. `Assets/Data/Turrets/Hummer.asset` (Projectile delivery) → `onHitModules`: добавить `Pierce.asset` и `Homing.asset`.
3. `Assets/Data/Turrets/Beam.asset` (Hitscan instant) → `onHitModules`: добавить `BeamSplit.asset`.
4. `Assets/Data/Turrets/Laser.asset` (Hitscan continuous) → `onHitModules`: добавить `LaserFocus.asset`.

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Pierce (Hummer): снаряд пробивает первого врага и летит дальше; урон следующего ×0.8; после 3 пробитий снаряд исчезает.
- [ ] Homing (Hummer): снаряд искривляет траекторию к ближайшему врагу; при спавне виден синий круг-маркер радиуса.
- [ ] Beam Split (Beam): луч при попадании разлетается на 3 луча (фиолетовый сектор-маркер + лучи); вложенность не зацикливается.
- [ ] Laser Focus (Laser): урон по одной цели растёт (HP проседает быстрее) до ×1.5; при смене цели сброс к базовому урону.
