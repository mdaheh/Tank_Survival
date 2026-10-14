# T037: DifficultyData.xpMultiplier
- Status: [ ]
- Phase: 6
- Files:
  - Assets/Scripts/Data/DifficultyData.cs

## Задача
В `DifficultyData` (namespace `TankSurvival.Data`) добавить публичное поле `public float xpMultiplier = 1f;` после существующих полей. Больше ничего не менять.

## Ограничения
- Единственный новый дефолт — `xpMultiplier = 1f`; остальные поля не трогать.
- namespace / `using UnityEngine` — как есть.

## Unity (человеку)
В существующих ассетах сложности выставить множитель по таблице «Множители врагов» в `Docs/GDD/Waves/Difficulties.md`:
- `Assets/Data/Difficulties/Recruit.asset` → `xpMultiplier` = 1.0.
- `Assets/Data/Difficulties/Sergeant.asset` → `xpMultiplier` = 1.2.
(Если есть другие DifficultyData — по той же таблице.)

## Проверка человеком
- `Recruit.asset` → xpMultiplier = 1; `Sergeant.asset` → xpMultiplier = 1.2.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
