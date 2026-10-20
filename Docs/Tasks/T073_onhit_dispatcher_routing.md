# T073: OnHitDispatcher — роутинг Slow/Freeze/Knockback
- Status: [ ]
- Phase: 10
- Depends: T066, T069, T070, T071
- Files:
  - Assets/Scripts/Combat/OnHitDispatcher.cs

## Задача
В `OnHitDispatcher` добавить поля `public SlowOnHitHandler slow;`, `public FreezeOnHitHandler freeze;`, `public KnockbackOnHitHandler knockback;`. В `ProcessHit(..., int depth)` добавить case'ы:
- `Slow`: `if (slow != null) slow.HandleHit(module, position, target); break;`
- `Freeze`: `if (freeze != null) freeze.HandleHit(module, position, target); break;`
- `Knockback`: `if (knockback != null) knockback.HandleHit(module, position, target); break;`

## Ограничения
- Не трогать case `Explosion`/`Burn`/`Ground` (Ground — из T072), сортировку `SetModules` и сигнатуру `ProcessHit`.
- namespace `TankSurvival.Combat` (хендлеры в том же namespace — `using` не добавлять).

## Unity (человеку)
1. На `Turret` → компонент `OnHitDispatcher`: поле `slow` → объект `SlowOnHitHandler`, `freeze` → `FreezeOnHitHandler`, `knockback` → `KnockbackOnHitHandler`.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
