# Phase 10: On-hit контроль — Freeze, Slow, Knockback, Ground
- Status: in-progress
- Цель: добавить 4 on-hit-модуля контроля поверх on-hit-слоя (IHitHandler/OnHitDispatcher): Slow, Freeze, Knockback, Ground. Вводится единая точка попадания `TryApplyHit` (guard живости), depth-counter (`max_depth`) против рекурсии Ground и конфликт `burn ↔ freeze`.

## Задачи (порядок = приоритет)
- T066: `OnHitModuleData` — kind Slow/Freeze/Knockback/Ground + поля slowAmount/freezeThreshold/force/damageScaling/groundVfx/maxDepth.
- T067: `EnemyController` — `controlSpeedMultiplier` + knockback (`ApplyKnockback`) + сброс в Init.
- T075: hit choke point — `TryApplyHit` + `ProcessHit(..., depth)` + перевод Cone/Hitscan/Projectile.
- T068: `ControlEffectSystem` — ApplySlow/ApplyFreeze/RemoveFreeze.
- T069: `SlowOnHitHandler`.
- T070: `FreezeOnHitHandler`.
- T071: `KnockbackOnHitHandler`.
- T072: `GroundOnHitHandler` + `GroundZone` + роутинг Ground (depth-guard).
- T073: `OnHitDispatcher` — роутинг Slow/Freeze/Knockback.
- T074: конфликт burn ↔ freeze (RemoveBurn/RemoveFreeze).

## Setup for smoke test (human)

Код не меняется — собираются данные и сцена (используются `OnHitModuleData`,
`ControlEffectSystem`, хендлеры, `GroundZone` из T066–T074).

Числа (GDD + Legend):
- Slow: `duration`=3, `slowAmount`=0.4, `priority`=4.
- Freeze: `duration`=3, `slowAmount`=0.6, `freezeThreshold`=3, `priority`=5.
- Knockback: `force`=3, `damageScaling`=0.5, `duration`=1, `priority`=3.
- Ground: `radius`=3, `duration`=3, `tickDamage`=1, `tickRate`=2, `priority`=7, `maxDepth`=5.

Шаги:
1. Project → `Assets/Data/OnHit` → Create → Tank Survival → OnHit Module Data:
   `Slow.asset`, `Freeze.asset`, `Knockback.asset`, `Ground.asset` (значения выше;
   неиспользуемые поля — 0/null/default; `groundVfx` в Ground → `GroundZoneVFX.prefab`
   из шага 2).
2. Project → `Assets/Prefabs/VFX` → создать префаб `GroundZoneVFX` (Plane/Cylinder,
   полупрозрачный материал) с компонентом `GroundZone`.
3. Hierarchy → пустые GameObject'ы `ControlEffectSystem`, `SlowOnHitHandler`,
   `FreezeOnHitHandler`, `KnockbackOnHitHandler`, `GroundOnHitHandler` → добавить
   одноимённые компоненты. Привязки: `SlowOnHitHandler.controlEffects` /
   `FreezeOnHitHandler.controlEffects` → `ControlEffectSystem`; `GroundOnHitHandler.pool`
   → `PoolManager`; `GroundOnHitHandler.onHit` → `OnHitDispatcher` (на `Turret`).
4. На тестовом `Turret` (Hammer): `TurretData.onHitModules` — по одному модулю за прогон
   (Slow / Freeze / Knockback / Ground, затем комбинации Ground+Slow, Burn+Freeze).
   `OnHitDispatcher` на `Turret`: привязать `slow`/`freeze`/`knockback`/`ground` к объектам
   из шага 3.
5. `SessionState.selectedTurret` → Hammer (как в Phase 5/9).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Slow (Hammer + Slow): враг после попадания движется медленнее (~×0.6) ~3 с, затем восстанавливается; повторное попадание обновляет длительность.
- [ ] Freeze (Hammer + Freeze): 1–2 стака замедляют (~×0.4); на 3-м стаке враг полностью останавливается на `duration`, затем восстанавливается.
- [ ] Knockback (Hammer + Knockback): попадание отталкивает врага от точки попадания, эффект затухает ~1 с.
- [ ] Ground (Hammer + Ground): в точке попадания зона; враги в радиусе получают урон тиками; зона исчезает через `duration`; число объектов не растёт (зона из пула).
- [ ] Ground + Slow: враги в зоне замедляются каждый тик (ре-триггер on-hit работает).
- [ ] Ground рекурсия ограничена `maxDepth`: при нескольких врагах в зоне нет вечного каскада/зависания, консоль чистая.
- [ ] Труп не источник on-hit (T075): мёртвый враг не триггерит on-hit — зона в позиции трупа не создаётся; взрыв/зона по убивающему выстрелу срабатывает.
- [ ] Конфликт burn/freeze: Burn+Freeze на одной турели — активен один (freeze снимает burn, burn снимает freeze).
- [ ] Регресс: Explosion/Burn и все delivery (Projectile / Hitscan instant / Hitscan continuous / Cone) не сломаны.
- [ ] Консоль чистая.
