# T126: Мост on-hit (Target.enemy + LegacyOnHitModule + ShotFactory)
- Status: [ ]
- Phase: 14
- Files:
  - Assets/Scripts/Combat/Shooting/Target.cs
  - Assets/Scripts/Combat/Shooting/LegacyOnHitModule.cs
  - Assets/Scripts/Combat/Shooting/ShotFactory.cs

## Задача
Сохранить текущие on-hit модули (Explosion/Burn/Slow/Freeze/Knockback/Ground) при переходе на новый пайплайн: они продолжают ходить через существующий `OnHitDispatcher` (миграция в модули — Phase 15).

1. В `Target` добавить поле `public EnemyController enemy;` и `using TankSurvival.Enemy;` (источник значения — контроллеры заполняют `target.enemy` при построении `Target` в T127/T128).
2. Создать `LegacyOnHitModule : IOnHit` в `TankSurvival.Combat.Shooting`:
   - лениво резолвить `OnHitDispatcher` через `GameServices.Get<OnHitDispatcher>()` (кэшировать, как в on-hit-обработчиках);
   - `OnHit(ShotContext ctx, Target target)`: если диспетчер и `target?.enemy` валидны — `dispatcher.ProcessHit(ctx.hitPoint, target.enemy, ctx.depth)`.
   - `using`: `TankSurvival.Combat` (`OnHitDispatcher`), `TankSurvival.Core` (`GameServices`), `TankSurvival.Enemy` (`EnemyController`).
3. В `ShotFactory.Build` заменить `hitModules = new IOnHit[] { new DamageModule() }` на `new IOnHit[] { new DamageModule(), new LegacyOnHitModule() }`.

## Ограничения
- `LegacyOnHitModule` stateless: не хранит состояние выстрела (кэш сервиса `OnHitDispatcher` допустим).
- Не менять логику `OnHitDispatcher` и его обработчиков.
- `ShotFactory` остаётся без GameObject/пула/визуала.

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T127–T130) урон наносится, и у турели с on-hit модулями срабатывают эффекты (Explosion-круг/DoT) как до фазы.

## Отчёт Исполнителя
Готово:
Блокер:
