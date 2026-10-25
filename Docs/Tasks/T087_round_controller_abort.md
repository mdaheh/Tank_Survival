# T087: RoundController — `isRoundActive` + `AbortRound()`
- Status: [x]
- Phase: 10.8
- Files:
  - Assets/Scripts/Round/RoundController.cs

## Задача
В `Assets/Scripts/Round/RoundController.cs` (namespace `TankSurvival.Round`) добавить состояние «раунд идёт» и метод досрочного завершения раунда без победы/поражения.

1. Рядом с публичными полями `phase`/`currentWave`/`kills` добавить публичное поле `public bool isRoundActive;`.
2. В методе `StartRound()`, после `kills = 0;`, добавить `isRoundActive = true;`.
3. В методе `HandleWaveCleared()`, в ветке `if (currentWave >= sessionState.SelectedDifficulty.waveCount)` перед `SetPhase(RoundPhase.Win);` добавить `isRoundActive = false;`.
4. В методе `HandlePlayerDeath()`, перед `SetPhase(RoundPhase.Lose);` добавить `isRoundActive = false;`.
5. Добавить метод:
   ```csharp
   public void AbortRound()
   {
       isRoundActive = false;
       waveSpawner.ClearActiveEnemies();
   }
   ```
   (`ClearActiveEnemies()` уже существует в `WaveSpawner` — останавливает спавн, очищает очередь и возвращает живых врагов в пул.)

## Ограничения
- Не менять `Restart()`, `StartRound()` (кроме добавленной строки), порядок вызовов и существующие события.
- Не трогать `Time.timeScale` здесь — пауза/выход управляют им отдельно.
- `AbortRound()` не меняет фазу на `Win`/`Lose` и не вызывает `OnPhaseChanged`.

## Unity (человеку)
Не требуется (поле `isRoundActive` появится в Inspector автоматически).

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → `RoundController` → `Is Round Active` = true во время идущего раунда; после победы (`phase = Win`) или поражения (`phase = Lose`) = false.
- (Действие `AbortRound()` проверяется в T092 кнопкой «Выйти» в паузе.)

## Отчёт Исполнителя
Готово: Assets/Scripts/Round/RoundController.cs: добавлено поле `public bool isRoundActive;`; в `StartRound()` после `kills = 0;` добавлено `isRoundActive = true;`; в `HandleWaveCleared()` в ветке победы и в `HandlePlayerDeath()` добавлено `isRoundActive = false;` перед `SetPhase(...)`; добавлен метод `AbortRound()` (`isRoundActive = false;` + `waveSpawner.ClearActiveEnemies();`).
Блокер: —
