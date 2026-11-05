# T138: Модули area (Explosion/Ground) — IOnHit + spawnQueue
- Status: [x]
- Phase: 15
- Depends: T134
- Files:
  - Assets/Scripts/Combat/Shooting/Modules/ExplosionModule.cs
  - Assets/Scripts/Combat/Shooting/Modules/GroundModule.cs

## Задача
Создать 2 stateless-модуля `IOnHit` в `Assets/Scripts/Combat/Shooting/Modules/` (namespace `TankSurvival.Combat.Shooting`), порождающих дочерний `ShotContext` через `ctx.spawnQueue` (по `Docs/Shooting.md` §9).

1. `ExplosionModule.OnHit(ShotContext ctx, Target target)`:
   - если `_data == null` — `return`;
   - `if (ctx.depth >= _data.maxDepth) return;` (guard глубины);
   - `BuildSnapshot child = ctx.buildSnapshot.Without<ExplosionModule>(); child.damage = _data.damage;`
   - `DeliveryConfig cfg; cfg.range = _data.radius; cfg.deathVfx = _data.explosionVfx;`
   - `SpawnRequest req = new SpawnRequest(); req.type = DeliveryType.AoE; req.position = ctx.hitPoint; req.direction = ctx.direction; req.buildSnapshot = child; req.config = cfg; req.depth = ctx.depth + 1;`
   - `ctx.spawnQueue.Add(req);`
2. `GroundModule.OnHit(ShotContext ctx, Target target)`:
   - если `_data == null` — `return`;
   - `if (ctx.depth >= _data.maxDepth) return;`
   - `BuildSnapshot child = ctx.buildSnapshot.Without<GroundModule>(); child.damage = _data.tickDamage;`
   - `DeliveryConfig cfg; cfg.range = _data.radius; cfg.tickRate = _data.tickRate; cfg.duration = _data.duration;`
   - `SpawnRequest req = new SpawnRequest(); req.type = DeliveryType.Zone; req.position = ctx.hitPoint; req.direction = ctx.direction; req.buildSnapshot = child; req.config = cfg; req.depth = ctx.depth + 1;`
   - `ctx.spawnQueue.Add(req);`

## Ограничения
- Модули stateless: конструктор принимает `OnHitModuleData`, поле `readonly`.
- Только данные — без GameObject/пула/визуала. `deathVfx`/`duration`/`tickRate`/`range` устанавливаются только здесь (T134).
- `using`: `TankSurvival.Data` (`OnHitModuleData`), `UnityEngine` (не требуется при нулевом направлении — оставить только нужные). Поля `SpawnRequest`/`BuildSnapshot`/`DeliveryConfig` — из `TankSurvival.Combat.Shooting` (текущий namespace).

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T135/T136/T139/T140/T141) взрыв и зона порождают дочерние area-выстрелы: взрыв — Terminal AoE-урон, зона — периодический урон в радиусе.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/Shooting/Modules/ExplosionModule.cs: `IOnHit` + `readonly OnHitModuleData _data`, guard `_data == null` и `ctx.depth >= _data.maxDepth`, `buildSnapshot.Without<ExplosionModule>()` + `child.damage = _data.damage`, `DeliveryConfig` (`range`, `deathVfx`) → `SpawnRequest` (`DeliveryType.AoE`, `hitPoint`, `direction`, `depth + 1`) в `ctx.spawnQueue`; Assets/Scripts/Combat/Shooting/Modules/GroundModule.cs: то же для зоны — `Without<GroundModule>()`, `child.damage = _data.tickDamage`, `DeliveryConfig` (`range`, `tickRate`, `duration`) → `SpawnRequest` (`DeliveryType.Zone`); `DeliveryConfig cfg = default;` вместо `DeliveryConfig cfg;` (иначе CS0165 — структура не definitely assigned)
Блокер: —
