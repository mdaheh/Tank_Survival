# T123: ConeController — конус (Periodic Hit)
- Status: [x]
- Phase: 13
- Depends: T119
- Files:
  - Assets/Scripts/Combat/Shooting/ConeController.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/Target.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Dev/DevVisualizer.cs

## Задача
Создать `ConeController` (class, stateless, namespace `TankSurvival.Combat.Shooting`) —
контроллер `DeliveryType.Cone` (Periodic Hit, Shooting.md §13):
`public void Tick(ShotContext ctx, float dt)`.

- Spawn-инициализация (в Tick, до ShotSystem) — как в T121.
- `origin = ctx.position; flatDir = ctx.direction; flatDir.y = 0f;`
- Periodic: пока `ctx.age < ctx.lifetime`, каждые `ctx.config.tickRate` (через `ctx.nextTickAt`)
  находить всех врагов в секторе (`Vector3.Angle(flatDir, toEnemy) <= ctx.config.angle * 0.5f`
  и `toEnemy.sqrMagnitude <= range²`) — как legacy `ConeDeliveryHandler`; на каждого собрать
  `Target` и вызвать `ctx.buildSnapshot.hitModules[].OnHit`; `keepAlive` игнорируется.
  По `lifetime` → Destroy.

Destroy — как в T121 (вызов `destroyModules`, `state = Dead`).

## Ограничения
- usings (полный блок): как в T121 (`TankSurvival.Enemy`, `UnityEngine`, `#if … TankSurvival.Dev`).
- Stateless; не двигает Transform.
- Dev-маркер (#if):
  `DevVisualizer.ShowSector(origin, flatDir, ctx.config.angle, ctx.config.range, 0.25f, new Color(1f, 0.55f, 0.1f, 0.25f))`
  (как legacy `ConeDeliveryHandler`).

## Unity (человеку)
Не требуется (класс не подключается в сцену).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: ConeController.cs: создан stateless-контроллер с Tick(ctx, dt) — Spawn→Flight, flatDir с обнулённым y, Periodic-тик по nextTickAt с шагом tickRate (проверка сектора Vector3.Angle ≤ angle*0.5 и sqrMagnitude ≤ range², hitModules[].OnHit на каждого врага), Destroy по lifetime через destroyModules, Dev-маркер ShowSector
Блокер: —
