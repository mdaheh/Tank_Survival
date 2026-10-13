# T019: SO-данные волн — WaveData + DifficultyData
- Status: [x]
- Phase: 3
- Files:
  - Assets/Scripts/Data/WaveData.cs
  - Assets/Scripts/Data/DifficultyData.cs

## Задача
Создать два ScriptableObject-класса в namespace `TankSurvival.Data`.
`WaveData` (`[CreateAssetMenu(menuName = "Tank Survival/Wave Data", fileName = "WaveData")]`):
поле `float spawnInterval = 2f` и `WaveComposition[] compositions`. Рядом (в том же файле
`WaveData.cs`) объявить `[System.Serializable] public struct WaveComposition` с полями
`EnemyData enemy`, `GameObject prefab`, `int baseCount`, `int increase`.
`DifficultyData` (`[CreateAssetMenu(menuName = "Tank Survival/Difficulty Data", fileName = "DifficultyData")]`):
поля `int waveCount = 3`, `float countMultiplier = 1f`, `float healthMultiplier = 1f`,
`float speedMultiplier = 1f`.

## Ограничения
- Оба класса в namespace `TankSurvival.Data`; `using UnityEngine`.
- `WaveComposition` — `[System.Serializable]`, объявлен в файле `WaveData.cs` (не отдельный файл).
- Множитель XP (`xpMultiplier`) в `DifficultyData` не добавлять — это фаза 6.
- Дефолтные значения только у перечисленных полей (`spawnInterval`, `waveCount`,
  `countMultiplier`, `healthMultiplier`, `speedMultiplier`); остальные — без дефолтов.

## Unity (человеку)
Создать папки и ассеты (ПКМ в папке → Create → Tank Survival → нужный тип):
- `Assets/Data/Waves/Wave1.asset` (WaveData): `spawnInterval` 1.5; `compositions` — 1 элемент:
  `enemy` → `Runner.asset`, `prefab` → `Runner.prefab`, `baseCount` 5, `increase` 2.
- `Assets/Data/Difficulties/Recruit.asset` (DifficultyData): `waveCount` 3, `countMultiplier` 0.6,
  `healthMultiplier` 0.75, `speedMultiplier` 0.9.
- `Assets/Data/Difficulties/Sergeant.asset` (DifficultyData): `waveCount` 3, `countMultiplier` 1,
  `healthMultiplier` 1, `speedMultiplier` 1.

## Проверка человеком
- Каждый ассет в инспекторе показывает перечисленные поля с введёнными значениями.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Data/WaveData.cs: класс WaveData + структура WaveComposition, Assets/Scripts/Data/DifficultyData.cs: класс DifficultyData
Блокер: —
