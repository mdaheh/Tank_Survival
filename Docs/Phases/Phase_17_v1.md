# Phase 17: Стрельба v2 — вражеский снаряд + зачистка legacy
- Цель: перевести вражеский снаряд (Shooter) на lifecycle-модель стрельбы (`ShotSystem` + фракционно-осведомлённый `ProjectileController`) и полностью удалить legacy-слой стрельбы/on-hit (`DeliveryHandler`, `IHitHandler`, `OnHitDispatcher`, legacy-обработчики, `EnemyProjectile`).

## Unity (человеку)
1. На объекте `Turret` (и, если остались, `Turret_Flamethrower`/`Turret_Laser`) удалить компоненты `ProjectileDeliveryHandler`, `HitscanDeliveryHandler`, `ConeDeliveryHandler` и `OnHitDispatcher` — их скрипты удалены, Unity покажет их как Missing Script. Если поле `On Hit` этих компонентов куда-то ссылалось — поле пропадёт вместе с компонентом.
2. На объекте `_Combat` удалить компоненты `SlowOnHitHandler`, `FreezeOnHitHandler`, `BurnOnHitHandler`, `KnockbackOnHitHandler`, `ExplosionOnHitHandler`, `GroundOnHitHandler` (скрипты удалены). Освободившиеся пустые GameObject'ы удалить.
3. Project → `Assets/Prefabs/Projectiles/Projectile.prefab` → удалить старый компонент `Projectile` (Missing Script), оставить `ShotView`.
4. Project → `Assets/Prefabs/Enemies/EnemyProjectile.prefab` → удалить префаб (больше не используется).
5. На объекте `ShotSystem` проверить: `Shot View Prefab` → `Projectile.prefab` (общий визуал и для снаряда игрока, и для вражеского).
6. На объекте `GameBootstrap` оставить `Projectile Prefab` → `Projectile.prefab` (преwarm того же пула). Поля `Enemy Projectile Prefab`/`Enemy Projectile Count` исчезли из инспектора автоматически.
7. Project → `Assets/Data/Enemies/Shooter.asset` → новое поле `Projectile Speed` = 10 (скорость вражеского снаряда; можно изменить).

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений (в т.ч. в сборке DEBUG — `DebugOverlayController` больше не ссылается на `GroundZone`).
- [x] Shooter стреляет снарядом в игрока: виден `ShotView`, при попадании игрок получает урон (`PlayerHealth`), снаряд исчезает; вне дистанции снаряд долетает до `shootRange * 2` и исчезает без урона.
- [x] Hammer (снаряд игрока) по-прежнему стреляет по врагам, наносит урон, враги умирают.
- [x] В сцене нет Missing Script от deleted legacy-компонентов.
- [x] Объекты не накапливаются: пул `ShotContext`/`ShotView`/VFX стабилен, активных снарядов не прибавляется.
- [x] Ground-зона (если задана у турели) по-прежнему тикает урон — визуально только debug-круг `DevVisualizer.ShowCircle`.

## Примечания
- Введена фракция (`Faction`): `SpawnRequest`/`ShotContext`/`Target` несут `faction`; `PlayerHealth` регистрируется в новом статическом `Combatants` как цель игрока; `ShotSystem` зарегистрирован в `GameServices`.
- `ProjectileController` стал фракционно-осведомлённым: `Faction.Player` → цели из `EnemyController.Active`, `Faction.Enemy` → игрок из `Combatants`. Остальные контроллеры (`Hitscan`/`Cone`/`AoE`/`Zone`) оставлены enemy-only — они используются только выстрелами игрока; вражеские лучи/зоны, если понадобятся, потребуют аналогичного расширения.
- Вражеский снаряд строится `ShotFactory.BuildProjectile(position, direction, damage, speed, range, faction)` и использует тот же `ShotView`-префаб, что и снаряд игрока (отдельный визуал не заводился).
- Модули, порождающие логических детей (`Explosion`/`Ground`/`BeamSplit`), пробрасывают `req.faction = ctx.faction` — дочерние выстрелы наследуют фракцию родителя.
- Визуал ground-зоны осознанно отложен (решение по фазе): legacy `GroundZone`/`GroundOnHitHandler` удалены, поле `OnHitModuleData.groundVfx` пока не читается новым `ZoneController` — проверка идёт debug-маркером.
- У `EnemyController` удалён `projectilePrefab` и параметр `PoolManager pool` из `Init` (использовался только для legacy-снаряда); `WaveSpawner` обновлён под новую сигнатуру.
