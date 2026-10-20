# T064: TurretController — пропуск мёртвых врагов при поиске цели
- Status: [ ]
- Phase: 9.5
- Depends: T063
- Files:
  - Assets/Scripts/Combat/TurretController.cs

## Задача
В цикле поиска ближайшего врага в `TurretController.Update` перед вычислением дистанции добавить пропуск мёртвых: `if (enemy.health == null || enemy.health.currentHealth <= 0f) continue;`. Логику поиска и выстрела не менять.

## Ограничения
- Менять только цикл поиска цели; маршрутизацию `delivery`/`hitscanDelivery`/`coneDelivery`, каденцию и `active.Fire(...)` не трогать.
- Проверка — именно по `enemy.health.currentHealth <= 0f`; новое поле/свойство не вводить.
- `using TankSurvival.Enemy` уже есть; namespace/`using` не менять.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
