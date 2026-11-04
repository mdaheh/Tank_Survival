# T107: OnHitDispatcher.GetModule — поиск модуля по kind
- Status: [ ]
- Phase: 11
- Depends: T106
- Files:
  - Assets/Scripts/Combat/OnHitDispatcher.cs

## Задача
В `OnHitDispatcher` добавить метод `public OnHitModuleData GetModule(OnHitModuleData.OnHitKind kind)` — линейный проход по `_modules`, вернуть первый модуль с `_modules[i].kind == kind`, иначе `null`. Без аллокаций (`foreach`/LINQ не использовать).

## Ограничения
- Не трогать `SetModules`, сортировку, `ProcessHit` и поля хендлеров.
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` уже есть.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
