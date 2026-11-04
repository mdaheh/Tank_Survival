# T127: Разделение hitscan (instant + continuous)
- Status: [ ]
- Phase: 14
- Depends: T126
- Files:
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/ShotFactory.cs
  - Assets/Scripts/Combat/Shooting/InstantHitscanController.cs
  - Assets/Scripts/Combat/Shooting/ContinuousHitscanController.cs
  - Assets/Scripts/Combat/Shooting/HitscanController.cs

## Задача
Развести hitscan на два типа/контроллера (`Shooting.md` §14.4): мгновенный (Terminal, без Flight) и непрерывный (Periodic, самонаводящийся). Убрать общий `HitscanController`.

1. `ShotTypes.cs`: в `DeliveryType` добавить значение `ContinuousHitscan` (существующий `Hitscan` остаётся = мгновенный).
2. `ShotFactory.cs`: `MapType` для `DeliveryModuleData.DeliveryKind.Hitscan` возвращать `ContinuousHitscan`, если `module.duration == DeliveryModuleData.DeliveryDuration.Continuous`, иначе `Hitscan`.
3. `InstantHitscanController` (новый): перенести логику старого `TickTerminal` — один raycast, до `maxTargets` целей вдоль луча, Terminal Hit, `keepAlive` учитывается, затем Destroy. Flight отсутствует (весь цикл в одном тике).
4. `ContinuousHitscanController` (новый): Periodic Hit, самонаведение:
   - Spawn → Flight. Каждый тик: найти ближайшую валидную цель в радиусе `config.range`;
   - нет цели → `destroyReason = External`, Destroy;
   - есть цель → `ctx.currentTarget = …`, `ctx.direction = normalize(target.position - ctx.position)`;
   - урон по `nextTickAt`/`tickRate`: при `age >= nextTickAt` цикл Hit (модули `OnHit` по целям вдоль луча до `maxTargets`, `OnPostHit` один раз), `nextTickAt += tickRate`; `keepAlive` игнорируется;
   - проверка срока `if (ctx.lifetime > 0f && ctx.age >= ctx.lifetime)`;
   - `target.enemy` заполнять при построении `Target`.
5. Удалить `HitscanController.cs` (логика разошлась по двум новым файлам).

## Ограничения
- Не менять `ProjectileController`, `ConeController` (это T128).
- Контроллеры stateless: без ссылки на `ShotSystem` (Destroy = `ctx.state = Dead`; финализацию/пулы делает `ShotSystem`).
- Debug-маркер луча оставить на кадр полёта (`DevVisualizer.ShowBeam`).

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T130) Laser бьёт непрерывно по ближайшей цели; при появлении более близкой перенацеливается; без цели — луч гаснет.

## Отчёт Исполнителя
Готово:
Блокер:
