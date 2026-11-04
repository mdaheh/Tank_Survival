# T117: IVfxSpawner — интерфейс спавна косметики
- Status: [ ]
- Phase: 12
- Files:
  - Assets/Scripts/Combat/Shooting/IVfxSpawner.cs

## Задача
Создать `IVfxSpawner` (namespace `TankSurvival.Combat.Shooting`) — интерфейс
спавна косметики (VFX); принадлежит контроллеру и НЕ входит в `ShotContext`
(Shooting.md §2, §10):
- `void Spawn(GameObject prefab, Vector3 position);`

## Ограничения
- Минимальная сигнатура; параметры (rotation/scale/pooling) уточнятся при проектировании VFX-слоя в следующей фазе.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
