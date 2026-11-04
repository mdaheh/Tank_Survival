# T122: HitscanController — мгновенный/непрерывный луч
- Status: [x]
- Phase: 13
- Depends: T119
- Files:
  - Assets/Scripts/Combat/Shooting/HitscanController.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/Target.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/ShotTypes.cs
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Dev/DevVisualizer.cs

## Задача
Создать `HitscanController` (class, stateless, namespace `TankSurvival.Combat.Shooting`) —
контроллер `DeliveryType.Hitscan`; один класс на оба режима (Architecture.md §7):
`public void Tick(ShotContext ctx, float dt)`.

- `origin = ctx.position; end = origin + ctx.direction * ctx.config.range;`
- Spawn-инициализация (в Tick, до ShotSystem) — как в T121: `state = Flight`, `age += dt`.
- `instant` (`ctx.config.continuous == false`, Terminal): один проход — собрать цели вдоль луча
  (дистанция точка→сегмент ≤ 1f), упорядочить по вдоль-лучу, взять до `ctx.config.maxTargets`
  первых не-мёртвых и не вошедших в `ctx.hitTargets`; на каждую собрать `Target` и вызвать
  `ctx.buildSnapshot.hitModules[].OnHit`; затем `keepAlive = false` → `postHitModules[].OnPostHit`;
  при `!keepAlive` → `destroyReason = Hit` → Destroy.
- `continuous` (`ctx.config.continuous == true`, Periodic): пока `ctx.age < ctx.lifetime`, каждые
  `ctx.config.tickRate` (через `ctx.nextTickAt`) повторять поиск вдоль луча и Hit; дедупликация по
  тикам (НЕ `hitTargets`), `keepAlive` игнорируется (Shooting.md §7.5, §13). По `lifetime` → Destroy.

Destroy — как в T121 (вызов `destroyModules`, `state = Dead`).

## Ограничения
- usings (полный блок): как в T121 (`TankSurvival.Enemy`, `UnityEngine`, `#if … TankSurvival.Dev`).
- Stateless; не двигает Transform.
- Dev-маркер (#if): `DevVisualizer.ShowBeam(origin, end, 0.15f, new Color(1f, 0.9f, 0.2f, 0.6f))`
  (как legacy `HitscanDeliveryHandler`).

## Unity (человеку)
Не требуется (класс не подключается в сцену).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: HitscanController.cs: создан stateless-контроллер с Tick(ctx, dt) — луч origin→origin+direction*range, Terminal-проход (до maxTargets целей по вдоль-лучу, дедуп по hitTargets, hitModules → keepAlive=false → postHitModules → Destroy по Hit) и Periodic-режим (по nextTickAt с шагом tickRate, дедуп внутри тика, Destroy по lifetime), Dev-маркер ShowBeam
Блокер: —
