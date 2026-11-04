# Phase 14: Стрельба v2 — интеграция + визуал
- Status: in-progress
- Цель: подключить lifecycle-слой Phases 12–13 к игре (`ShotSystem`, `ShotView`, `VfxSpawner`), развести мгновенный и непрерывный hitscan, перевести `TurretController` на `ShotFactory`, сохранив поведение текущей стрельбы.

## Задачи (порядок = приоритет)
- T124: `ShotView` — визуальная оболочка выстрела из пула.
- T125: `VfxSpawner` — реализация `IVfxSpawner` (+ направление).
- T126: Мост on-hit (`Target.enemy` + `LegacyOnHitModule` + подключение в `ShotFactory`).
- T127: Разделение hitscan (`DeliveryType.ContinuousHitscan` + `InstantHitscanController`/`ContinuousHitscanController`).
- T128: Непрерывное самонаведение и контракт `lifetime` (`ConeController`, `ProjectileController`, `ShotContext`).
- T129: `ShotSystem` — оркестратор (`Fire → id`, `IsAlive`, пул контекстов, finalize).
- T130: `TurretController` → `ShotFactory` + `ShotSystem` (инициация вместо пере-выстрела).
- T131: `ShotView` — убрать предупреждение сериализации (UAC1001).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Hammer: снаряд летит (виден `ShotView`), попадает во врага, враг получает урон и умирает.
- [ ] Laser (continuous hitscan): луч непрерывно бьёт ближайшую цель; при появлении более близкой перенацеливается; без цели в радиусе гаснет.
- [ ] Flamethrower (cone): непрерывно бьёт всех в конусе, пока есть цель; `ConeVFX` проигрывается.
- [ ] On-hit модули (Explosion/Burn/…), если заданы у турели, срабатывают как до фазы.
- [ ] Объекты не накапливаются: активные снаряды/VFX возвращаются в пул (счётчик объектов стабилен).

## Примечания
- Непрерывная доставка = один автономный выстрел (см. `Shooting.md` §14): сам наводится, тикает `tickRate`, живёт до `External` (нет цели). Турель только инициирует; `lifetime` необязателен.
- Pellet/spread не мигрируются (базовые турели — 1 pellet, без spread) — отложено до Phase 18.
- Legacy-обработчики (`DeliveryHandler`, `Projectile`, `*DeliveryHandler`) остаются в коде, но `TurretController` их больше не использует; зачистка — Phase 17.
- Путь конфига `DeliveryModuleData → ShotFactory → SpawnRequest.config`; копирование `SpawnRequest`-полей в `ShotContext` выполняется на стадии Spawn в `ShotSystem` (T129).
