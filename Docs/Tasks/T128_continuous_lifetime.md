# T128: Непрерывное самонаведение (Cone) + контракт lifetime
- Status: [ ]
- Phase: 14
- Depends: T126
- Files:
  - Assets/Scripts/Combat/Shooting/ConeController.cs
  - Assets/Scripts/Combat/Shooting/ProjectileController.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs

## Задача
Завершить контракт непрерывной (Periodic) доставки и правило `lifetime`.

1. `ShotContext.cs`:
   - в `Reset()` ставить `lifetime = -1f;`
   - задокументировать поле `lifetime`: «-1 = без таймера; > 0 = секунды жизни»;
   - уточнить комментарий `currentTarget`: для точечной доставки — жертва; для площадной — якорь направления (урон всем из `FindTargets`).
2. `ProjectileController.cs`:
   - в Spawn-ветке ставить `ctx.lifetime = ctx.buildSnapshot.speed > 0f ? ctx.config.range / ctx.buildSnapshot.speed : -1f;`
   - проверку срока сменить на `if (ctx.remainingRange <= 0f || (ctx.lifetime > 0f && ctx.age >= ctx.lifetime))`;
   - заполнять `target.enemy = enemy;`.
3. `ConeController.cs`: сделать непрерывным самонаводящимся (Periodic, `Shooting.md` §14):
   - Spawn → Flight. Каждый тик искать ближайшую валидную цель в радиусе `config.range`;
   - нет цели → `destroyReason = External`, Destroy;
   - есть цель → `ctx.currentTarget` = ближайшая (якорь направления), `ctx.direction = normalize(target.position - ctx.position)` (плоская, y=0);
   - урон по `nextTickAt`/`tickRate`: при `age >= nextTickAt` обойти всех в секторе (`angle`/`range`), Hit-цикл, `nextTickAt += tickRate`; `keepAlive` игнорируется;
   - проверка срока `if (ctx.lifetime > 0f && ctx.age >= ctx.lifetime)`;
   - заполнять `target.enemy = enemy;`.

## Ограничения
- Не менять `InstantHitscanController` / `ContinuousHitscanController` (T127) и `ShotSystem` (T129).
- Cone VFX в контроллере не проигрывать (это делает `ShotSystem` через `IVfxSpawner`).
- Debug-маркер сектора (`DevVisualizer.ShowSector`) оставить.

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T130) Flamethrower непрерывно бьёт всех в конусе, пока есть цель; без цели — гаснет.

## Отчёт Исполнителя
Готово:
Блокер:
