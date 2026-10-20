# T065: ExplosionOnHitHandler — убрать снапшот после Deferred Release
- Status: [ ]
- Phase: 9.5
- Depends: T063
- Files:
  - Assets/Scripts/Combat/ExplosionOnHitHandler.cs

## Задача
После Deferred Release (T062–T063) `EnemyController.Active` стабилен в игровой логике, и снапшот в `ExplosionOnHitHandler` больше не нужен. Убрать поле `_buffer` и его заполнение (`_buffer.Clear(); _buffer.AddRange(EnemyController.Active);`); в `HandleHit` итерировать `EnemyController.Active` напрямую. Удалить `using System.Collections.Generic;` (после правки `List<>` в файле не используется).

## Ограничения
- Итерация — прямой `for (int i = 0; i < EnemyController.Active.Count; i++)`, без `foreach`/LINQ/`GetComponent` в цикле.
- Логику радиуса (`radiusSq`), нанесения урона и спавна VFX не менять.
- `_buffer` и все обращения к нему (`Clear`, `AddRange`, `Count`, `[i]`) удалить полностью.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
