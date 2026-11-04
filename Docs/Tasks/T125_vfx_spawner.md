# T125: VfxSpawner — реализация IVfxSpawner (+ направление)
- Status: [x]
- Phase: 14
- Files:
  - Assets/Scripts/Combat/Shooting/IVfxSpawner.cs
  - Assets/Scripts/Combat/Shooting/VfxSpawner.cs

## Задача
1. Расширить `IVfxSpawner` (в `TankSurvival.Combat.Shooting`): добавить перегрузку
   `void Spawn(GameObject prefab, Vector3 position, Vector3 direction);`
   Существующий `void Spawn(GameObject prefab, Vector3 position);` остаётся.
2. Создать `VfxSpawner : MonoBehaviour, IVfxSpawner` — спавн косметики через `PoolManager`:
   - в `Awake` зарегистрировать себя: `GameServices.Register<IVfxSpawner>(this)`.
   - `PoolManager` получать лениво через `GameServices.Get<PoolManager>()` (кэшировать в приватное поле, как в существующих on-hit-обработчиках).
   - `Spawn(prefab, position)` → делегировать в `Spawn(prefab, position, Vector3.zero)`.
   - `Spawn(prefab, position, direction)` → если `prefab == null || pool == null` — выйти; `GameObject vfx = pool.Get(prefab)`; `vfx.transform.position = position`; при `direction.sqrMagnitude > 1e-6f` — `Vector3 fwd = direction; fwd.y = 0f; vfx.transform.rotation = Quaternion.LookRotation(fwd)`; `vfx.GetComponent<PooledVFX>().Play(pool)`.

## Ограничения
- Не создаёт `ShotContext`; VFX — чистая косметика без контрактов.
- `using`: `TankSurvival.Core` (`PoolManager`, `GameServices`), `TankSurvival.Combat` (`PooledVFX`), `UnityEngine`.
- Не трогает визуал выстрела (`ShotView`) и логику выстрела.
- Ноль `Instantiate` в `Update` (спавн — только через `PoolManager.Get`).

## Unity (человеку)
1. В сцене создать пустой GameObject с именем `VfxSpawner` → Add Component → `VfxSpawner`.
   (Регистрация в `GameServices` происходит в `Awake`, поля в инспекторе не требуются.)

## Проверка человеком
- Hierarchy → объект `VfxSpawner` с компонентом `VfxSpawner`.
- В Play Mode (после T130) `ConeVFX` проигрывается при выстреле Flamethrower и самостоятельно гаснет.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/Shooting/IVfxSpawner.cs: добавлена перегрузка Spawn(GameObject, Vector3, Vector3), старая сигнатура сохранена; Assets/Scripts/Combat/Shooting/VfxSpawner.cs: создан VfxSpawner : MonoBehaviour, IVfxSpawner с регистрацией в GameServices.Awake, ленивым кэшем PoolManager, делегированием Spawn(prefab, position) → Spawn(..., Vector3.zero), спавном через pool.Get, установкой позиции/поворота по direction и запуском PooledVFX.Play(pool)
Блокер: —
