# T133: Cone VFX следует за непрерывным конусом
- Status: [ ]
- Phase: 14
- Depends: T132
- Files:
  - Assets/Scripts/Combat/Shooting/ConeController.cs
  - Assets/Scripts/Combat/Shooting/ShotSystem.cs
  - Assets/Scripts/Combat/Shooting/ShotFactory.cs
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs

## Задача
Сейчас конусный VFX спавнится один раз в `ShotSystem.Fire` (поле `coneVfxPrefab`)
и дальше не двигается и не повторяется: у непрерывного огнемёта визуал «отстаёт»
от турели и играет одну вспышку, хотя урон продолжается. Перенести спавн VFX в
`ConeController`, чтобы он следовал за текущей точкой выстрела на каждом
damage-тике.

1. `DeliveryConfig.cs`: добавить поле `public GameObject coneVfx;` (добавить `using UnityEngine;`).
2. `ShotFactory.cs`: в `Build` заполнить `config.coneVfx = module.coneVfx;`.
3. `ConeController.cs`:
   - добавить `using TankSurvival.Core;`;
   - добавить кэшируемое приватное поле `IVfxSpawner _vfx` и ленивое свойство (как
     `PoolService`/`VfxService` в `ShotSystem`):
     `get { if (_vfx == null) _vfx = GameServices.Get<IVfxSpawner>(); return _vfx; }`;
   - внутри damage-тика (`if (ctx.age >= ctx.nextTickAt)`, после
     `ctx.nextTickAt += ctx.config.tickRate;` и `RunSectorHits(...)`) добавить:
     ```
     IVfxSpawner vfx = Vfx;
     if (ctx.config.coneVfx != null && vfx != null)
     {
         vfx.Spawn(ctx.config.coneVfx, ctx.position, ctx.direction);
     }
     ```
     Использовать текущие `ctx.position`/`ctx.direction` (их обновляют T132 и
     самонаведение), чтобы VFX следовал за конусом.
4. `ShotSystem.cs`: убрать спавн конуса из `Fire` и ставшие ненужными части:
   - удалить поле `[SerializeField] private GameObject coneVfxPrefab;`;
   - удалить поле `private IVfxSpawner _vfx;` и свойство `VfxService` (остаётся только `PoolService`);
   - удалить `case DeliveryType.Cone:` (блок с `VfxService.Spawn(coneVfxPrefab, ctx.position, ctx.direction);`) из `switch (request.type)` в `Fire`.

## Ограничения
- `IVfxSpawner`/`VfxSpawner` не менять: используется существующий `Spawn(prefab, position, direction)`.
- Без аллокаций в `Tick`: `GameServices.Get` — только при первом обращении (кэш в поле).
- VFX не создаёт `ShotContext`; конусный VFX остаётся самоисчезающей косметикой (`PooledVFX` с `duration`), спавнится заново каждый тик.
- `InstantHitscanController` / `ContinuousHitscanController` / `ProjectileController` не менять.

## Unity (человеку)
- На объекте `ShotSystem` поле `Cone Vfx Prefab` больше не используется и исчезнет из инспектора — заполнять его не нужно. VFX берётся из `delivery.coneVfx` (поле `Cone Vfx` в `DeliveryModuleData`), которое уже настроено для огнемёта.

## Проверка человеком
- Play Mode (Editor): Flamethrower — конусный VFX появляется, следует за турелью/направлением при движении и повороте, продолжается, пока есть цель, и перестаёт появляться, когда цель вышла из радиуса (после затухания последнего всплеска).
- VFX не накапливается: объекты конуса возвращаются в пул (`PooledVFX` саморелизуется по `duration`).

## Отчёт Исполнителя
Готово:
Блокер:
