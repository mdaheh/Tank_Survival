# T047: SessionState — selectedDifficulty
- Status: [ ]
- Phase: 7
- Files:
  - Assets/Scripts/Core/SessionState.cs

## Задача
В `SessionState` (namespace `TankSurvival.Core`) добавить выбор сложности рядом с `selectedChassis`/`selectedTurret`:
- `[SerializeField] private DifficultyData selectedDifficulty;`
- `public DifficultyData SelectedDifficulty => selectedDifficulty;`

## Ограничения
- Только хранение выбора; без логики старта/сброса раунда.
- `DifficultyData` — `TankSurvival.Data`; `using TankSurvival.Data;` уже есть в файле.

## Unity (человеку)
- GameObject `GameBootstrap` → компонент `SessionState` → поле `selectedDifficulty` → `Assets/Data/Difficulties/Recruit.asset` (тот же ассет, что сейчас стоит в поле `difficulty` компонента `RoundController`).

## Проверка человеком
- Инспектор `SessionState`: `selectedChassis`, `selectedTurret`, `selectedDifficulty` заполнены.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
