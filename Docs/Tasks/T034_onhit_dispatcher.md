# T034: OnHitDispatcher — маршрутизация on_hit по priority
- Status: [ ]
- Phase: 5
- Depends: T032, T033
- Files:
  - Assets/Scripts/Combat/OnHitDispatcher.cs

## Задача
Создать `OnHitDispatcher : MonoBehaviour` (namespace `TankSurvival.Combat`). Поля: `public ExplosionOnHitHandler explosion`, `public BurnOnHitHandler burn`; приватный `List<OnHitModuleData> _modules`. `public void SetModules(OnHitModuleData[] modules)`: очистить `_modules`, скопировать не-null элементы, отсортировать по `priority` по убыванию (сортировка вставками, без LINQ/аллокаций). `public void ProcessHit(Vector3 position, EnemyController target)`: для каждого модуля — `Explosion` → `explosion.HandleHit(...)`, `Burn` → `burn.HandleHit(...)` (null-проверки на handler'ы).

## Ограничения
- `SetModules` вызывается раз (из `TurretController.Start`), `ProcessHit` — при каждом попадании: в `ProcessHit` нет `new`, LINQ, `GetComponent`.
- namespace `TankSurvival.Combat`; `using TankSurvival.Data` (OnHitModuleData), `using TankSurvival.Enemy` (EnemyController), `using UnityEngine`, `using System.Collections.Generic`.

## Unity (человеку)
Не требуется (компонент добавляется на турели в T036).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
