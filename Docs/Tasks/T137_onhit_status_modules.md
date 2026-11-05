# T137: Модули статусов/контроля (Burn/Slow/Freeze/Knockback)
- Status: [ ]
- Phase: 15
- Files:
  - Assets/Scripts/Combat/Shooting/Modules/BurnModule.cs
  - Assets/Scripts/Combat/Shooting/Modules/SlowModule.cs
  - Assets/Scripts/Combat/Shooting/Modules/FreezeModule.cs
  - Assets/Scripts/Combat/Shooting/Modules/KnockbackModule.cs

## Задача
Создать 4 stateless-модуля `IOnHit` в `Assets/Scripts/Combat/Shooting/Modules/` (namespace `TankSurvival.Combat.Shooting`). Каждый хранит конфиг `OnHitModuleData` (передаётся в конструктор, `readonly` поле) и лениво кэширует сервисы через `GameServices.Get` (как `LegacyOnHitModule`). Всё состояние остаётся во владении `StatusEffectSystem`/`ControlEffectSystem`.

1. `BurnModule.OnHit(ShotContext ctx, Target target)`:
   - если `target?.enemy == null` — `return`;
   - `ControlEffectSystem` → `RemoveFreeze(target.enemy)`;
   - `StatusEffectSystem` → `ApplyBurn(target.enemy, data.tickDamage, data.tickRate, data.duration)`.
2. `SlowModule.OnHit`:
   - если `target?.enemy == null` — `return`;
   - `ControlEffectSystem` → `ApplySlow(target.enemy, data.slowAmount, data.duration)`.
3. `FreezeModule.OnHit`:
   - если `target?.enemy == null` — `return`;
   - `StatusEffectSystem` → `RemoveBurn(target.enemy)`;
   - `ControlEffectSystem` → `ApplyFreeze(target.enemy, data.slowAmount, data.duration, data.freezeThreshold)`.
4. `KnockbackModule.OnHit`:
   - если `target?.enemy == null || target.enemy.health == null || target.enemy.health.currentHealth <= 0f` — `return`;
   - `Vector3 dir = target.enemy.transform.position - ctx.hitPoint; dir.y = 0f; dir.Normalize();`
   - `target.enemy.ApplyKnockback(dir * (data.force * data.damageScaling), data.duration);`.

## Ограничения
- Модули stateless: только `readonly OnHitModuleData _data` + кэш сервисов (допустимо, как в legacy-обработчиках). Никаких per-shot полей.
- Без аллокаций в `OnHit` (кроме первого ленивого резолва сервисов).
- `using`: `TankSurvival.Data` (`OnHitModuleData`), `TankSurvival.Enemy` (`EnemyController`), `TankSurvival.Core` (`GameServices`), `TankSurvival.Combat` (`StatusEffectSystem`, `ControlEffectSystem`), `UnityEngine` (`Vector3`).

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T139/T141) статусы работают как legacy: burn тикает уроном, freeze замораживает (стаки до `freezeThreshold`), slow замедляет, knockback отталкивает от точки попадания.

## Отчёт Исполнителя
Готово:
Блокер:
