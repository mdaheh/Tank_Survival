# T025: WaveSpawner — уход от magic-метода Reset
- Status: [x]
- Phase: 3
- Depends: T021, T022
- Files:
  - Assets/Scripts/Waves/WaveSpawner.cs
  - Assets/Scripts/Round/RoundController.cs

## Задача
В классе `WaveSpawner` переименовать метод `public void Reset()` в `public void ClearActiveEnemies()` и навесить на него атрибут `[ContextMenu("Release Active Enemies")]`. Тело метода не менять. В классе `RoundController` в методе `HandlePlayerDeath()` заменить вызов `waveSpawner.Reset();` на `waveSpawner.ClearActiveEnemies();`.

## Ограничения
- Не менять логику (тело) метода `ClearActiveEnemies`.
- Не трогать другие методы и публичные поля в обоих файлах.
- Не добавлять `Instantiate`/`Destroy` и `SceneManager.LoadScene`.
- Имя метода `Reset` должно исчезнуть полностью (уход от встроенного magic-метода Unity).

## Unity (человеку)
не требуется

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- В инспекторе `WaveSpawner` → `⋮` появился пункт `Release Active Enemies`; пункт `Reset`, вызывающий рантайм-сброс, отсутствует (остаётся только встроенный `Reset`, сбрасывающий поля инспектора).
- Запустить Play Mode (волна 1 активна) → на `WaveSpawner` нажать `⋮ → Release Active Enemies` → активные враги деактивируются (исчезают со сцены), новый спавн не идёт.
- Проверить, что поля `waveData`/`difficulty`/`spawnPoints`/`poolManager`/`playerTransform` в инспекторе `WaveSpawner` после нажатия `Release Active Enemies` не обнулились.

## Отчёт Исполнителя
Готово: Assets/Scripts/Waves/WaveSpawner.cs: Reset → ClearActiveEnemies + ContextMenu("Release Active Enemies"), Assets/Scripts/Round/RoundController.cs: вызов в HandlePlayerDeath обновлён
Блокер: —
