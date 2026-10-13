# T021: WaveSpawner + SpawnPoint — спавн волн
- Status: [x]
- Phase: 3
- Depends: T019, T020
- Files:
  - Assets/Scripts/Waves/SpawnPoint.cs
  - Assets/Scripts/Waves/WaveSpawner.cs

## Задача
Создать два класса в namespace `TankSurvival.Waves`.
`SpawnPoint` (MonoBehaviour): маркер точки спавна — только `OnDrawGizmos()` с
`Gizmos.DrawWireSphere(transform.position, 0.5f)` (цвет любой).
`WaveSpawner` (MonoBehaviour) с сериализуемыми полями `WaveData waveData`,
`DifficultyData difficulty`, `SpawnPoint[] spawnPoints`, `PoolManager poolManager`,
`Transform playerTransform`; `event Action OnWaveCleared`. Приватное: `List<EnemyController> _active`,
приватная вложенная `struct SpawnOrder { public EnemyData enemy; public GameObject prefab; }` и
`List<SpawnOrder> _queue`, `int _spawnIndex`, `float _timer`, `bool _spawning`.
Методы:
- `public void BeginWave(int waveNumber)`: очистить очередь и вернуть активных врагов в пул
  (`poolManager.Release`), затем построить очередь: для каждой `composition` из
  `waveData.compositions` посчитать `count = Mathf.RoundToInt((composition.baseCount +
  (waveNumber - 1) * composition.increase) * difficulty.countMultiplier)` и добавить `count`
  записей `(composition.enemy, composition.prefab)`; `_timer = 0`, `_spawning = true`.
- `private void Update()`: если `!_spawning` — return; если `_queue.Count > 0`:
  `_timer -= Time.deltaTime`; при `_timer <= 0` — спавнить одного врага и `_timer = waveData.spawnInterval`.
- спавн одного врага: взять следующую запись из `_queue`, `GameObject go = poolManager.Get(prefab)`,
  `go.transform.position = spawnPoints[_spawnIndex % spawnPoints.Length].position`, `_spawnIndex++`;
  получить `EnemyController controller = go.GetComponent<EnemyController>()`; присвоить
  `controller.health.healthMultiplier = difficulty.healthMultiplier` и
  `controller.speedMultiplier = difficulty.speedMultiplier`; подписаться на смерть ПЕРЕД init:
  `controller.health.OnDeath += () => HandleEnemyDeath(controller);`
  затем `controller.Init(enemy, playerTransform, poolManager)`; добавить `controller` в `_active`.
- `private void HandleEnemyDeath(EnemyController controller)`: `_active.Remove(controller)`;
  если `_queue.Count == 0` и `_active.Count == 0` → `_spawning = false; OnWaveCleared?.Invoke();`.
- `public void Reset()`: `_spawning = false`, очистить `_queue`, `_timer = 0`, вернуть всех врагов
  из `_active` в пул (проход с конца по копии: `poolManager.Release(controller.gameObject)`), `_active.Clear()`.
- `public void Prewarm()`: для каждой `composition` вычислить максимум врагов за волну
  (`max` по `n = 1..difficulty.waveCount` той же формулы) и вызвать `poolManager.Prewarm(prefab, max)`.
- `[ContextMenu("Begin Wave 1")] public void BeginWave1()` → `BeginWave(1)`.

## Ограничения
- Подписка на `EnemyHealth.OnDeath` выполняется ДО `EnemyController.Init` (декремент счётчика
  должен идти раньше возврата в пул).
- `Instantiate`/`Destroy` не вызываются; инстансы — только через `poolManager.Get`/`Release`.
- `GetComponent` только при спавне (не каждый кадр); без LINQ и аллокаций в `Update`.
- namespace `TankSurvival.Waves`; `using TankSurvival.Core` (PoolManager), `using TankSurvival.Data`
  (WaveData/DifficultyData/EnemyData), `using TankSurvival.Enemy` (EnemyController/EnemyHealth),
  `using System` (Action), `using System.Collections.Generic`.

## Unity (человеку)
В `Assets/Scenes/Main.unity`:
1. Создать 4 пустых объекта `SpawnPoint_1..4` по краям карты; на каждый добавить компонент
   `SpawnPoint` (Add Component → SpawnPoint).
2. Создать пустой GameObject `WaveSpawner`; добавить компонент `WaveSpawner`.
3. Заполнить поля `WaveSpawner`: `waveData` → `Wave1.asset`, `difficulty` → `Sergeant.asset`,
   `spawnPoints` → 4 объекта SpawnPoint, `poolManager` → `PoolManager`, `playerTransform` → игрок.

## Проверка человеком
- Проект компилируется без ошибок.
- В инспекторе `WaveSpawner`: `Context Menu (⋮) → Begin Wave 1` — враги спавнятся по одному с
  интервалом `spawnInterval` из точек `SpawnPoint_1..4`; после зачистки новых спавнов нет.
- `Context Menu (⋮) → Reset` — активные враги деактивируются, число объектов в иерархии
  возвращается к исходному.

## Отчёт Исполнителя
Готово: Assets/Scripts/Waves/SpawnPoint.cs: маркер с OnDrawGizmos, Assets/Scripts/Waves/WaveSpawner.cs: BeginWave/Update/SpawnNext/HandleEnemyDeath/Reset/Prewarm/BeginWave1
Блокер: —
