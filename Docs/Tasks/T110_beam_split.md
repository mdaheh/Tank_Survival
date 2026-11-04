# T110: Beam Split — разлёт hitscan-луча
- Status: [-]
- Phase: 11
- Depends: T107
- Files:
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs

## Задача
Hitscan-луч с модулем Beam Split при попадании разлетается на `splitCount` лучей из точки попадания, распределённых по углу `splitAngle`; каждый вложенный луч наследует тот же `damage` и on-hit-цепочку, глубина ограничена `module.maxDepth`. Реализовать в `HitscanDeliveryHandler`:
1. Рефакторинг `Fire`:
   - `public override void Fire(Vector3 direction, float damage)` → `FireBeam(muzzle.position, direction, damage, 0);`
   - Новый `public void FireBeam(Vector3 origin, Vector3 direction, float damage, int depth)`: `Vector3 end = origin + direction * module.range;` → `DevVisualizer.ShowBeam(origin, end, ...)` (как сейчас) → `GatherCandidates(origin, end);` → `int toHit = Mathf.Min(module.maxTargets, _candidates.Count);` → цикл `for (i < toHit) _candidates[i].TryApplyHit(damage, _candidates[i].transform.position, onHit, depth);` → затем разлёт (п.2).
   - Новый `private void GatherCandidates(Vector3 origin, Vector3 end)` — перенести сюда текущий код сбора (`_candidates.Clear()`, перебор `EnemyController.Active` по `DistanceToSegmentXZSqr(enemy.position, origin, end) <= HitRadiusSqr`) и сортировку ближайших вдоль луча (`AlongBeamXZ`).
2. В `FireBeam` после цикла попаданий: `OnHitModuleData split = onHit != null ? onHit.GetModule(OnHitModuleData.OnHitKind.BeamSplit) : null;` — если `split != null && depth < split.maxDepth`, то для каждого из `toHit` попаданий (`Vector3 hitPos = _candidates[i].transform.position;`) сгенерировать `split.splitCount` направлений: `float angleStep = split.splitCount > 1 ? split.splitAngle / (split.splitCount - 1) : 0f;` для `k in [0, splitCount)`: `float angle = -split.splitAngle * 0.5f + angleStep * k; Vector3 splitDir = Quaternion.AngleAxis(angle, Vector3.up) * direction;` и `FireBeam(hitPos, splitDir, damage, depth + 1);`
3. Отладочный маркер: при разлёте — `#if UNITY_EDITOR || DEBUG DevVisualizer.ShowSector(hitPos, direction, split.splitAngle, module.range, 0.3f, new Color(0.6f, 0.4f, 1f, 0.4f)); #endif` (параметры сектора из `split.splitAngle` и `module.range`); лучи рисуются уже существующим `ShowBeam` внутри `FireBeam`.

## Ограничения
- Рекурсия ограничена `depth < split.maxDepth` (`maxDepth` — поле OnHitModuleData, дефолт 5). Логику `maxTargets` не менять.
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` и `using TankSurvival.Dev` (под `#if`) уже есть.

## Unity (человеку)
1. `Assets/Data/OnHit/` → создать `BeamSplit.asset`: `kind = BeamSplit`, `splitCount = 3`, `splitAngle = 45`, `priority = 6`.
2. `Assets/Data/Turrets/Beam.asset` (Hitscan instant) → `onHitModules` добавить `BeamSplit.asset`.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Dev Menu → турель Beam → Start Round. Луч попадает во врага и разлетается на 3 луча (виден фиолетовый сектор-маркер + лучи). Вложенность не уходит в бесконечную рекурсию (нет зависания/переполнения).

## Отчёт Исполнителя
Готово:
Блокер:
