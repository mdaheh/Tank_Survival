# T115: BuildSnapshot — набор модулей и статов выстрела
- Status: [ ]
- Phase: 12
- Depends: T114
- Files:
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs

## Задача
Создать `BuildSnapshot` (namespace `TankSurvival.Combat.Shooting`, Shooting.md §11, §9):
- массивы модулей: `IOnSpawn[] spawnModules; IOnFlight[] flightModules; IOnHit[] hitModules; IOnPostHit[] postHitModules; IOnDestroy[] destroyModules;`
- базовые статы: `float damage; float speed;` (источник §11).
- `BuildSnapshot Without<T>()` — копия снимка без модулей типа `T` (избирательное наследование §9).
- `static BuildSnapshot Empty` — пустой снимок (изоляция §9).

## Ограничения
- `Without<T>`/`Empty` используются при спавне дочерней сущности, не в горячем цикле полёта; аллокация копии допустима.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
