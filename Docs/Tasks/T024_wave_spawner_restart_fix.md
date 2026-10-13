# T024: WaveSpawner — сброс активных врагов в BeginWave
- Status: [x]
- Phase: 3
- Depends: T021
- Files:
  - Assets/Scripts/Waves/WaveSpawner.cs

## Задача
В методе `BeginWave(int waveNumber)` класса `WaveSpawner` после цикла, который возвращает активных врагов в пул (`for (int i = _active.Count - 1; i >= 0; i--) { poolManager.Release(_active[i].gameObject); }`), добавить две строки: `_active.Clear();` и `_spawnIndex = 0;` — перед блоком построения очереди из `waveData.compositions`.

## Ограничения
- Не трогать `Reset()`, `Prewarm()`, `Update()`, `SpawnNext()`, `HandleEnemyDeath()`, `BeginWave1()`.
- Не менять сигнатуры методов и публичные поля класса.
- Не добавлять `Instantiate`/`Destroy`.
- Порядок существующих операций в `BeginWave` (очистка `_queue`, возврат в пул, построение очереди, `_timer = 0f`, `_spawning = true`) сохранить.

## Unity (человеку)
не требуется

## Проверка человеком
- Проект компилируется без ошибок.
- Запустить Play Mode, дождаться активной волны 1 (враги на сцене), затем на объекте `RoundController` нажать `Inspector → ⋮ → Restart Round`.
- Убедиться: `RoundController` (Inspector) → `currentWave = 1`, `phase = Playing`.
- Убить всех врагов волны 1 → `currentWave` становится 2, спавнится волна 2.
- Довести до зачистки волны 3 → `phase = Win`.
- Повторить рестарт на живой 3-й волне: раунд проходит до `Win` без застревания на волне 1.

## Отчёт Исполнителя
Готово: Assets/Scripts/Waves/WaveSpawner.cs: в BeginWave после возврата врагов в пул добавлены _active.Clear() и _spawnIndex = 0
Блокер: —
