# T029: OnHitModuleData + TurretData.onHitModules
- Status: [ ]
- Phase: 5
- Files:
  - Assets/Scripts/Data/OnHitModuleData.cs
  - Assets/Scripts/Data/TurretData.cs

## Задача
Создать `OnHitModuleData : ScriptableObject` (namespace `TankSurvival.Data`, `[CreateAssetMenu(menuName = "Tank Survival/OnHit Module Data")]`) с вложенным `enum OnHitKind { Explosion, Burn }` и полями: `OnHitKind kind`, `int priority`, `float radius` (Explosion), `float damage` (Explosion), `GameObject explosionVfx` (Explosion, префаб), `float duration` (Burn), `float tickDamage` (Burn), `float tickRate` (Burn). В `TurretData` добавить публичное поле `OnHitModuleData[] onHitModules` (массив, по образцу `WaveComposition[] compositions` в `WaveData`).

## Ограничения
- Существующее поле `TurretData.delivery` не трогать.
- namespace `TankSurvival.Data`; `[CreateAssetMenu]` — как у соседних SO.
- `enum OnHitKind` менять нельзя (используется в T032–T034).

## Unity (человеку)
Создать ассеты (ПКМ → Create → Tank Survival → OnHit Module Data):
1. `Assets/Data/OnHit/Explosion.asset`: `kind = Explosion`, `priority = 10`, `radius = 3`, `damage = 9`; `explosionVfx` оставить пустым (назначить в T033).
2. `Assets/Data/OnHit/Burn.asset`: `kind = Burn`, `priority = 5`, `duration = 3`, `tickDamage = 1`, `tickRate = 0.5`; `radius`/`damage`/`explosionVfx` = 0/пусто (не используются).
3. `Assets/Data/Turrets/Hammer.asset` → в `onHitModules` добавить `Explosion.asset`.
4. `Assets/Data/Turrets/Beam.asset` → в `onHitModules` добавить `Burn.asset`.

Числа: `Explosion.md` (radius medium, damage high, priority 10) → `Legend.md` §1.1 (medium = 3, high = 9); `Burn.md` (tick_damage low, tick_rate medium, priority 5) → `Legend.md` §1.1 (low = 1), §1.3 (medium = 0.5 с). Для `duration` в `Legend.md` шкалы нет — берём `medium → 3 с` как MVP-значение.

## Проверка человеком
- `Explosion.asset`: kind=Explosion, priority=10, radius=3, damage=9.
- `Burn.asset`: kind=Burn, priority=5, duration=3, tickDamage=1, tickRate=0.5.
- `Hammer.asset` → onHitModules = [Explosion.asset]; `Beam.asset` → onHitModules = [Burn.asset].
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
