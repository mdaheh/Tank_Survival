# T139: ShotFactory — сборка on-hit модулей из OnHitModuleData[]
- Status: [x]
- Phase: 15
- Depends: T137, T138
- Files:
  - Assets/Scripts/Combat/Shooting/ShotFactory.cs

## Задача
Заменить мост `LegacyOnHitModule` на сборку настоящих on-hit модулей из `OnHitModuleData[]`.

1. Добавить перегрузку:
   `public SpawnRequest Build(DeliveryModuleData module, OnHitModuleData[] onHitModules, Vector3 position, Vector3 direction, float damage, int depth = 0)`.
2. Старую сигнатуру `Build(DeliveryModuleData module, Vector3 position, Vector3 direction, float damage, int depth = 0)` оставить как делегат: `return Build(module, Array.Empty<OnHitModuleData>(), position, direction, damage, depth);` (добавить `using System;`).
3. В новой `Build`:
   - собрать список on-hit модулей в порядке приоритета: отфильтровать `null` из `onHitModules`, отсортировать по `priority` строго по убыванию (стабильный insertion sort, как в `OnHitDispatcher.SetModules`);
   - сформировать `IOnHit[]` размером `1 + N`: `[0] = new DamageModule()`, далее — по одному модулю на `OnHitModuleData` через маппинг `kind`:
     `Burn → new BurnModule(data)`, `Slow → new SlowModule(data)`, `Freeze → new FreezeModule(data)`, `Knockback → new KnockbackModule(data)`, `Explosion → new ExplosionModule(data)`, `Ground → new GroundModule(data)`;
   - `buildSnapshot.hitModules = этот массив;` — убрать `new LegacyOnHitModule()`.
   - Остальную сборку (`damage`, `speed`, `DeliveryConfig`, `MapType`) не менять.

## Ограничения
- Без GameObject/пула/визуала.
- `using TankSurvival.Data` (`OnHitModuleData`, `DeliveryModuleData`) уже есть; добавить `using System` (для `Array.Empty`).
- Порядок hit-модулей: всегда `DamageModule` первым, затем on-hit по убыванию `priority`.
- `LegacyOnHitModule` больше не создаётся в `Build` (файл остаётся в коде, не используется).

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно с T141) on-hit эффекты срабатывают через новые модули; legacy `OnHitDispatcher` не вызывается.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/Shooting/ShotFactory.cs: добавлена перегрузка `Build(DeliveryModuleData, OnHitModuleData[], Vector3, Vector3, float, int = 0)`, старая сигнатура делегирует с `Array.Empty<OnHitModuleData>()` (добавлен `using System;`, `using System.Collections.Generic;`), `buildSnapshot.hitModules = BuildHitModules(onHitModules)` — `DamageModule` первым + модули по `kind` (Burn/Slow/Freeze/Knockback/Explosion/Ground), `null` отфильтрован и приоритеты отсортированы стабильным insertion sort по убыванию `priority` (как в `OnHitDispatcher.SetModules`), `new LegacyOnHitModule()` убран
Блокер: —
