# T124: ShotView — визуальная оболочка выстрела
- Status: [ ]
- Phase: 14
- Files:
  - Assets/Scripts/Combat/Shooting/ShotView.cs

## Задача
Создать `ShotView : MonoBehaviour, IPoolable` в `TankSurvival.Combat.Shooting` — тонкий визуал выстрела из пула. Каждый кадр читает `ctx.position`/`ctx.direction` и двигает Transform. Логику и переходы FSM не хранит.

Поля:
- `public ShotContext ctx;` — контекст, который визуал отображает (источник: присваивается методом `Bind` из `ShotSystem`).

Методы:
- `public void Bind(ShotContext context)` — сохранить ссылку на контекст и сразу применить `transform.position = ctx.position`.
- `private void Update()` — если `ctx != null`: `transform.position = ctx.position`; выровнять поворот по направлению: `Vector3 fwd = ctx.direction; fwd.y = 0f;` при `fwd.sqrMagnitude > 1e-6f` → `transform.rotation = Quaternion.LookRotation(fwd)`.
- `OnGetFromPool()` / `OnReleaseFromPool()` (контракт `IPoolable`) — `ctx = null`.

## Ограничения
- Без логики: не трогает `ctx.state`, не вызывает модули, не обращается к `PoolManager`.
- `using` только `TankSurvival.Core` (для `IPoolable`) и `UnityEngine`.
- Не создаёт GameObject, не проигрывает VFX.

## Unity (человеку)
1. Project → `Assets/Prefabs/Projectiles/Projectile.prefab` → Add Component → `ShotView`.
   - Старый компонент `Projectile` на префабе оставить (он безвреден: `Update` выходит по `pool == null`); сам скрипт зачищается в Phase 17.

## Проверка человеком
- Inspector → на `Assets/Prefabs/Projectiles/Projectile.prefab` есть компонент `ShotView`.
- В Play Mode (после T130) снаряд виден, движется к цели; при уничтожении GameObject деактивируется (вернулся в пул).

## Отчёт Исполнителя
Готово:
Блокер:
