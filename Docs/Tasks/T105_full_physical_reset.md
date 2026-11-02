# T105: Полный физический сброс рестарта (PlayerSpawnPoint + ReleaseAllActive)
- Status: [x]
- Phase: 10.10
- Files:
  - Assets/Scripts/Player/PlayerSpawnPoint.cs
  - Assets/Scripts/Core/PoolManager.cs
  - Assets/Scripts/Round/RoundController.cs

## Задача
Сделать «возврат в меню» и «старт раунда» полным сбросом: вернуть игрока на точку спавна и вернуть в пул все активные pooled-объекты (снаряды игрока/врага, VFX, GroundZone), чтобы рестарт выглядел как новый вход в Play Mode.

1. Создать `Assets/Scripts/Player/PlayerSpawnPoint.cs` (маркер точки спавна игрока, по образцу `SpawnPoint`):
```csharp
using UnityEngine;

namespace TankSurvival.Player
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
```

2. `Assets/Scripts/Core/PoolManager.cs` — добавить метод `ReleaseAllActive()` после `Release(...)`:
```csharp
public void ReleaseAllActive()
{
    foreach (GameObject instance in _instancePrefabs.Keys)
    {
        if (instance != null && instance.activeSelf)
        {
            Release(instance);
        }
    }
}
```
(итерировать `_instancePrefabs.Keys` безопасно: `Release` не изменяет `_instancePrefabs`, а guard `instance != null` и `activeSelf` пропускает неактивные/уничтоженные.)

3. `Assets/Scripts/Round/RoundController.cs`:
   - добавить поле `[SerializeField] private PlayerSpawnPoint playerSpawnPoint;` (после `progression`);
   - добавить приватный метод:
```csharp
private void ResetPlayerToSpawn()
{
    if (playerSpawnPoint == null || waveSpawner.playerTransform == null)
    {
        return;
    }

    waveSpawner.playerTransform.SetPositionAndRotation(
        playerSpawnPoint.transform.position,
        playerSpawnPoint.transform.rotation);
}
```
   - в `ResetToMenu()` после `waveSpawner.ClearActiveEnemies();` добавить `waveSpawner.poolManager.ReleaseAllActive();` и `ResetPlayerToSpawn();` (итог: сначала очистить врагов, затем остальные активные pooled-объекты, затем позицию);
   - в `StartRound()` после `playerHealth.Init();` добавить `ResetPlayerToSpawn();`, а после `waveSpawner.BeginWave(currentWave);` добавить `waveSpawner.poolManager.ReleaseAllActive();` (после `BeginWave`, т.к. `BeginWave` сам возвращает активных врагов в пул — иначе `ReleaseAllActive` вызовет двойной Release по T101-guard).

## Ограничения
- `ReleaseAllActive()` полагается на T101-guard: неактивные инстансы не релизятся повторно. Порядок в `StartRound`: `BeginWave` (освобождает врагов) → `ReleaseAllActive` (освобождает остальное).
- `waveSpawner.playerTransform` и `waveSpawner.poolManager` — существующие публичные поля `WaveSpawner` (уже используются в `SpawnNext`/`ClearActiveEnemies`), новых ссылок не требуют.
- `RoundController` уже имеет `using TankSurvival.Player;` — `PlayerSpawnPoint` доступен без нового using.
- `ResetToMenu()` по-прежнему не меняет `phase` и не вызывает `OnPhaseChanged` (иначе снова покажется RoundEndUI).

## Unity (человеку)
- В сцене создать пустой GameObject с именем `PlayerSpawnPoint` на стартовой позиции игрока (позиция и поворот = желаемое место появления игрока) и добавить компонент `PlayerSpawnPoint`. Разместить в иерархии (например, под группой `_SpawnPoints` или `_Player`).
- На компоненте `RoundController` заполнить новое поле `Player Spawn Point` этим GameObject.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu → Start Round → отъехать от стартовой позиции, выстрелить и умереть → экран «Поражение» → клик по кнопке → Dev Menu открыт; игрок стоит на точке `PlayerSpawnPoint`, выпущенный снаряд исчез (в иерархии нет активных `Projectile`/`EnemyProjectile`/VFX вне пула).
- «Start Round» → игрок начинает с точки спавна, враги спавнятся штатно, нет «фантомного» урона от старых снарядов.
- Тот же результат при выходе из паузы («Выйти»).

## Отчёт Исполнителя
Готово: Assets/Scripts/Player/PlayerSpawnPoint.cs: создан маркер `PlayerSpawnPoint` с gizmo-сферой; Assets/Scripts/Core/PoolManager.cs: добавлен `ReleaseAllActive()` после `Release(...)`; Assets/Scripts/Round/RoundController.cs: добавлено поле `playerSpawnPoint`, метод `ResetPlayerToSpawn()`, в `ResetToMenu()` после `ClearActiveEnemies()` вставлены `ReleaseAllActive()` и `ResetPlayerToSpawn()`, в `StartRound()` после `playerHealth.Init()` — `ResetPlayerToSpawn()`, после `BeginWave(currentWave)` — `ReleaseAllActive()`.
Блокер: —
