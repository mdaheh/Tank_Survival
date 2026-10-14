# T036: TurretController — привязка on_hit-слоя
- Status: [ ]
- Phase: 5
- Depends: T034, T035
- Files:
  - Assets/Scripts/Combat/TurretController.cs

## Задача
В `TurretController` добавить `public OnHitDispatcher onHit;`. Добавить `private void Start()`: если `onHit != null` — `if (turret != null) onHit.SetModules(turret.onHitModules);` и привязать диспетчер к delivery-хендлерам: `if (delivery != null) delivery.onHit = onHit; if (hitscanDelivery != null) hitscanDelivery.onHit = onHit;`. Существующую логику `Update` (поиск цели, выбор handler'а по `turret.delivery.kind`, кулдаун) не менять.

## Ограничения
- `SetModules` вызывать один раз (в `Start`), не в `Update`.
- namespace `TankSurvival.Combat`; новые `using` не нужны (OnHitDispatcher — тот же namespace, `turret.onHitModules` — через `TankSurvival.Data`).

## Unity (человеку)
1. На объекте `Turret` (Hammer): добавить компонент `OnHitDispatcher`; в нём `explosion` → объект `ExplosionOnHitHandler`, `burn` → объект `BurnOnHitHandler`. В `TurretController` поле `onHit` → компонент `OnHitDispatcher` (на этом же объекте).
2. На объекте `Turret_Beam`: добавить компонент `OnHitDispatcher`; `explosion` → `ExplosionOnHitHandler`, `burn` → `BurnOnHitHandler`; `TurretController.onHit` → этот `OnHitDispatcher`.

## Проверка человеком
- Инспектор `Turret` → `TurretController.onHit` = OnHitDispatcher; `OnHitDispatcher.explosion/burn` заполнены.
- Инспектор `Turret_Beam` → то же самое.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
