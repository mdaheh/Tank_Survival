# T116: ShotContext — контейнер состояния выстрела
- Status: [ ]
- Phase: 12
- Depends: T113, T115
- Files:
  - Assets/Scripts/Combat/Shooting/ShotContext.cs

## Задача
Создать `ShotContext` (class, не MonoBehaviour, namespace
`TankSurvival.Combat.Shooting`) — контейнер всего состояния выстрела от Spawn до
Dead. Поля (Shooting.md §4.2, §5):
- общие: `ShotState state; float age; float lifetime; bool keepAlive; DestroyReason destroyReason; int depth; BuildSnapshot buildSnapshot; List<SpawnRequest> spawnQueue; List<Target> hitTargets; Vector3 hitPoint;`
- специфичные типа доставки: `Vector3 position; Vector3 direction; float speed; Vector3 prevPosition; float remainingRange; float tickRate; float nextTickAt; Target currentTarget;`
- метод `void Reset()` — обнуляет перечисленные поля в дефолт (`state = Dead`, числа в 0/false, списки `Clear()`, ссылки в null). Кастомный сброс — через `IResettable` (§5).

## Ограничения
- Без ссылок на GameObject/Transform/prefab (инвариант §14.12).
- Списки переиспользуются (`Clear()`), без `new` в горячем цикле.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
