# T111: Laser Focus — накопление урона continuous-луча
- Status: [ ]
- Phase: 11
- Depends: T110
- Files:
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs

## Задача
Continuous hitscan с модулем Laser Focus накапливает урон по цели, пока она остаётся в луче: каждый тик стак +1 (до `maxStacks`), итоговый урон тика = `damage × (1 + damageGrowth × stacks)`; при смене цели стаки сбрасываются. Реализовать в `HitscanDeliveryHandler`:
1. Новые приватные поля: `private EnemyController _focusTarget;`, `private int _focusStacks;`.
2. В `Fire` (до вызова `FireBeam`):
   - `OnHitModuleData focus = onHit != null ? onHit.GetModule(OnHitModuleData.OnHitKind.LaserFocus) : null;`
   - если `focus != null && module != null && module.duration == DeliveryModuleData.DeliveryDuration.Continuous`:
     - `Vector3 end = muzzle.position + direction * module.range;`
     - `GatherCandidates(muzzle.position, end);`
     - `EnemyController target = _candidates.Count > 0 ? _candidates[0] : null;`
     - `if (target != _focusTarget) { _focusTarget = target; _focusStacks = 0; }`
     - `if (_focusTarget != null) { _focusStacks = Mathf.Min(_focusStacks + 1, focus.maxStacks); damage *= 1f + focus.damageGrowth * _focusStacks; }`
   - затем `FireBeam(muzzle.position, direction, damage, 0);`
3. Отладочный маркер: маркер — уже существующая линия луча `DevVisualizer.ShowBeam` в `FireBeam` (отражает активность луча). Стаки наблюдаются по ускорению просадки `currentHealth` врага (Debug Overlay → блок «Враг»). Дополнительных пространственных маркеров нет (механика времени, без радиуса/угла).

## Ограничения
- `_focusTarget`/`_focusStacks` применяются только на верхнем уровне (путь через `Fire`, `depth == 0`); вложенные лучи Beam Split (`FireBeam` с `depth > 0`) Laser Focus не применяют.
- Без `new`/LINQ. `FireBeam`/`GatherCandidates` из T110 не менять (кроме использования `GatherCandidates` в `Fire`).
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` уже есть.

## Unity (человеку)
1. `Assets/Data/OnHit/` → создать `LaserFocus.asset`: `kind = LaserFocus`, `damageGrowth = 0.1`, `maxStacks = 5`, `stackResetOnSwitch = 1`, `priority = 9`.
2. `Assets/Data/Turrets/Laser.asset` (Hitscan continuous) → `onHitModules` добавить `LaserFocus.asset`.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Dev Menu → турель Laser → Start Round. Удерживать луч на одном враге: его HP проседает всё быстрее (урон растёт по стакам до ×1.5). Перевести луч на другого врага → накопление сброшено (урон снова базовый).

## Отчёт Исполнителя
Готово:
Блокер:
