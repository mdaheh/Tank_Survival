# Phase 17.5: Стрельба v2 — закрытие инвариантов

- Цель: привести lifecycle-слой стрельбы в полное соответствие `Docs/Shooting.md`: убрать
  ссылки на `EnemyController` (MonoBehaviour) из `ShotContext`/`Target` (ключ статусных
  систем — `IDamageable`), устранить аллокации в горячем цикле (`Target`/`SpawnRequest`
  → `struct`), задействовать `ctx.speed` и вызывать `IOnPostHit` после периодического
  Hit в `ConeController`/`ZoneController`.

## Unity (человеку)

не требуется — правки только в коде (`.cs`), новых компонентов и полей в инспекторе нет.
Достаточно скомпилировать проект.

## Smoke-тест (проверяет человек)

- [x] Проект компилируется без ошибок и предупреждений.
- [x] Hammer (Projectile) наносит урон врагам; с Pierce снаряд пробивает несколько целей,
      урон снижается по falloff.
- [x] On-hit статусы работают как раньше: Slow замедляет, Freeze замораживает (после
      порога стаков), Burn тикает урон, Knockback отталкивает от точки попадания
      (`ctx.hitPoint`), Burn снимает Freeze, Freeze снимает Burn.
- [x] Continuous Hitscan / Cone непрерывно тикают урон (`nextTickAt`), наводятся на цель
      и исчезают при отсутствии цели (`External`). Ground-зона тикает урон (debug-круг).
- [x] Объекты не накапливаются: пулы `ShotContext`/`ShotView`/VFX стабильны, активных
      снарядов не прибавляется.
- [x] (DEBUG) в Debug Overlay блок «Враг» по-прежнему показывает Slow/Frozen/Burn/
      Knockback у активного врага.

## Примечания

- Статусные системы (`StatusEffectSystem`, `ControlEffectSystem`) теперь принимают
  `IDamageable` и резолвят `EnemyController` через статический реестр
  `EnemyController.TryGetByDamageable` (наполнение в `OnEnable`/`OnDisable`). Интерфейс
  `IDamageable` дополнен свойством `bool IsAlive`.
- `Target` и `SpawnRequest` переведены из `class` в `struct` — из горячего цикла ушло
  `new Target()`/`new SpawnRequest()` (struct — на стеке). Осталась одна известная
  аллокация `BuildSnapshot.Without<T>()` на событии Hit в Explosion/Ground (низкая
  частота), вынесена за рамки фазы.
- `ctx.speed` задействован: инициализируется из `buildSnapshot.speed` на Spawn
  (`ShotSystem.Fire`), используется `ProjectileController` для `velocity`/`lifetime`.
- В `ConeController`/`ZoneController` добавлен вызов `IOnPostHit` после периодического
  Hit — симметрия с `ContinuousHitscanController` (`Docs/Shooting.md` §4.1/§6.4).
- Laser Focus отслеживает смену цели по `IDamageable` (`ctx.laserFocusedTarget`) вместо
  `ctx.laserFocusedEnemy`.