# T120: ShotFactory — сборка SpawnRequest из DeliveryModuleData
- Status: [ ]
- Phase: 13
- Depends: T119
- Files:
  - Assets/Scripts/Combat/Shooting/ShotFactory.cs
  - Assets/Scripts/Combat/Shooting/SpawnRequest.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Combat/Shooting/DamageModule.cs
  - Assets/Scripts/Data/DeliveryModuleData.cs

## Задача
Создать `ShotFactory` (class, namespace `TankSurvival.Combat.Shooting`, Shooting.md §7,
Architecture.md §7) — чистую фабрику, собирающую `SpawnRequest`:
- `public SpawnRequest Build(DeliveryModuleData module, Vector3 position, Vector3 direction,
  float damage, int depth = 0)`.

Сборка:
- `type`: маппинг `DeliveryModuleData.DeliveryKind → DeliveryType`
  (`Projectile`/`Hitscan`/`Cone`; `AoE` здесь не порождается).
- `buildSnapshot`: `damage = damage` (аргумент, НЕ `module.damage` — эффективный урон
  приходит параметром), `speed = module.speed`, `hitModules = new IOnHit[] { new DamageModule() }`,
  остальные массивы (`spawnModules`/`flightModules`/`postHitModules`/`destroyModules`) — пустые
  (`Array.Empty<T>()` или `null`, по конвенции T115).
- `config`: `range = module.range`, `angle = module.angle`, `maxTargets = module.maxTargets`,
  `continuous = module.duration == DeliveryModuleData.DeliveryDuration.Continuous`,
  `tickRate = module.tickRate`.
- `position`/`direction`/`depth` — из аргументов.

## Ограничения
- `usings`: `using TankSurvival.Data;` и `using UnityEngine;`.
- Фабрика stateless; не создаёт GameObject, не трогает пул/визуал (это Phase 14).

## Unity (человеку)
Не требуется (не подключается в сцену).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
