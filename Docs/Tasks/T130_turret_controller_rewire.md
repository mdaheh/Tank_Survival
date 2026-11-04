# T130: TurretController → ShotFactory + ShotSystem
- Status: [ ]
- Phase: 14
- Depends: T129
- Files:
  - Assets/Scripts/Combat/TurretController.cs

## Задача
Переключить стрельбу турели на новый lifecycle: инициация выстрелов вместо пере-выстрела.

Изменения:
1. Поля: убрать `delivery`, `hitscanDelivery`, `coneDelivery`; добавить `public ShotSystem shotSystem;`. Оставить `turret`, `onHit`, `statBlock`. Добавить `private readonly ShotFactory _factory = new ShotFactory();` и `private int _continuousShotId = -1;`.
2. `Start()`: оставить только `onHit.SetModules(turret.onHitModules)`.
3. `Update()`:
   - `bool continuous = turret.delivery.duration == DeliveryModuleData.DeliveryDuration.Continuous;`
   - **Continuous**: если `continuous && turret.delivery.tickRate <= 0f` — выйти. Если `_continuousShotId == -1 || !shotSystem.IsAlive(_continuousShotId)` — найти ближайшего врага в радиусе (`turret.delivery.range`); если есть — `Vector3 dir = (closest.position - position).normalized; _continuousShotId = shotSystem.Fire(_factory.Build(turret.delivery, position, dir, statBlock.damage));`. Кулдаун не используется.
   - **Instant/Projectile**: кулдаун как раньше (`interval = 1f / statBlock.fireRate`); при `cooldown <= 0` и есть ближайшая цель — `shotSystem.Fire(_factory.Build(turret.delivery, position, dir, statBlock.damage)); cooldown = interval;` (id не сохранять).

## Ограничения
- Не менять логику наведения (ближайший враг в радиусе) и кулдаун для instant/projectile.
- Добавить `using TankSurvival.Combat.Shooting;` (`ShotSystem`, `ShotFactory`, `SpawnRequest`).
- Pellet/spread не реализуются (базовые турели — 1 pellet) — Phase 18.

## Unity (человеку)
1. Объект `Turret` → компонент `TurretController`:
   - поле `Shot System` → из Hierarchy объект `ShotSystem`;
   - поля `Delivery`, `Hitscan Delivery`, `Cone Delivery` — очистить;
   - поле `On Hit` и `Stat Block` — оставить как было.
2. Legacy-компоненты `ProjectileDeliveryHandler`/`HitscanDeliveryHandler`/`ConeDeliveryHandler` на `Turret` оставить (не используются) — зачистка Phase 17.

## Проверка человеком
- Inspector → объект `Turret` → `TurretController` → `Shot System` указывает на `ShotSystem`, поля delivery очищены.
- Play Mode: Hammer стреляет снарядом; Laser/Flamethrower бьют непрерывно и гаснут без цели.

## Отчёт Исполнителя
Готово:
Блокер:
