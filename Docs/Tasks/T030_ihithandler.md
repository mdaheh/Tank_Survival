# T030: IHitHandler — контракт on_hit-модуля
- Status: [x]
- Phase: 5
- Depends: T029
- Files:
  - Assets/Scripts/Combat/IHitHandler.cs

## Задача
Создать интерфейс `IHitHandler` (namespace `TankSurvival.Combat`) с единственным методом `void HandleHit(OnHitModuleData module, Vector3 position, EnemyController target)`. `using TankSurvival.Data` (OnHitModuleData), `using TankSurvival.Enemy` (EnemyController), `using UnityEngine` (Vector3).

## Ограничения
- Только интерфейс, без реализации и без MonoBehaviour.
- namespace и сигнатуру метода не менять (используются в T032–T034).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: IHitHandler.cs: создан интерфейс IHitHandler (namespace TankSurvival.Combat) с методом HandleHit(OnHitModuleData, Vector3, EnemyController)
Блокер: —
