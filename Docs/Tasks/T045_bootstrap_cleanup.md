# T045: GameBootstrap — финальная чистка
- Status: [x]
- Phase: 6
- Depends: T044
- Files:
  - Assets/Scripts/Core/GameBootstrap.cs

## Задача
В `GameBootstrap` (namespace `TankSurvival.Core`):
- удалить вызов `statBlock.Init(sessionState.SelectedChassis, sessionState.SelectedTurret);` из `Start` (теперь пересчёт статов делает `RoundController.StartRound` в T044).
- удалить ставшие неиспользуемыми сериализуемые поля `statBlock` и `sessionState`.
- оставить поля `poolManager`, `projectilePrefab`, `projectileCount`, `waveSpawner`, `roundController` и тело `Start`: `poolManager.Prewarm(projectilePrefab, projectileCount); waveSpawner.Prewarm(); roundController.StartRound();`.

## Ограничения
- Без `SceneManager.LoadScene`; prewarm — только в `Start`.
- `using TankSurvival.Round`, `using TankSurvival.Waves` — как есть; лишние `using` убрать (если остались только от удалённых полей).

## Unity (человеку)
- На GameObject `GameBootstrap`: после компиляции поля `statBlock` и `sessionState` исчезнут из инспектора — это ожидаемо, ничего дозаполнять не нужно. Проверить, что `poolManager`/`projectilePrefab`/`projectileCount`/`waveSpawner`/`roundController` подключены.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode: раунд стартует (`currentWave = 1`), статы игрока считаются из `SelectedChassis`/`SelectedTurret` (в `StatBlock`), убийства дают XP (см. Smoke-тест фазы 6).

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/GameBootstrap.cs — удалён вызов statBlock.Init из Start и поля statBlock/sessionState; оставлены poolManager/projectilePrefab/projectileCount/waveSpawner/roundController и prewarm в Start.
Блокер: —
