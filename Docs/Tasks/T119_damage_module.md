# T119: DamageModule — единая точка нанесения урона
- Status: [x]
- Phase: 13
- Depends: T114, T116
- Files:
  - Assets/Scripts/Combat/Shooting/DamageModule.cs
  - Assets/Scripts/Combat/Shooting/ShotContracts.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs
  - Assets/Scripts/Combat/Shooting/BuildSnapshot.cs
  - Assets/Scripts/Combat/Shooting/Target.cs
  - Assets/Scripts/Core/IDamageable.cs

## Задача
Создать `DamageModule` (class, namespace `TankSurvival.Combat.Shooting`) — stateless-модуль,
реализующий `IOnHit` и наносящий прямой урон цели (Shooting.md §6.3, Architecture.md §7):
- `public void OnHit(ShotContext ctx, Target target)` вызывает
  `target.damageable.TakeDamage(ctx.buildSnapshot.damage);` с guard на
  `target == null` и `target.damageable == null`.

Это единственная точка нанесения прямого урона в lifecycle-модели — заменяет вызов
`IDamageable.TakeDamage` напрямую из контроллеров.

## Ограничения
- Stateless: модуль ничего не хранит, не управляет переходами, не трогает `ctx.state`.
- Тип `IDamageable` в этом файле НЕ объявляется напрямую: вызов идёт через поле
  `target.damageable.TakeDamage(...)`, поэтому `using TankSurvival.Core;` НЕ нужен.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: DamageModule.cs: создан stateless-модуль DamageModule : IOnHit, OnHit с guard на target/target.damageable и вызовом target.damageable.TakeDamage(ctx.buildSnapshot.damage)
Блокер: —
