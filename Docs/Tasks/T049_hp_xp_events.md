# T049: PlayerHealth/ProgressionSystem — события HP/XP для HUD
- Status: [ ]
- Phase: 7
- Files:
  - Assets/Scripts/Player/PlayerHealth.cs
  - Assets/Scripts/Progression/ProgressionSystem.cs

## Задача
1. `PlayerHealth` (namespace `TankSurvival.Player`): добавить `public event Action<float, float> OnHealthChanged;`.
   - в `Init()` после `currentHealth = statBlock.maxHealth;` добавить `OnHealthChanged?.Invoke(currentHealth, statBlock.maxHealth);`;
   - в `TakeDamage()` после `currentHealth -= amount;` добавить `OnHealthChanged?.Invoke(currentHealth, statBlock.maxHealth);` (до проверки смерти).
2. `ProgressionSystem` (namespace `TankSurvival.Progression`): добавить `public event Action OnProgressChanged;` и `public float XpNeeded => 5f + level * 3f;`.
   - в `AddXp()`: после `xp += amount;` добавить `OnProgressChanged?.Invoke();`; после `level++;` (внутри ветки level-up) добавить `OnProgressChanged?.Invoke();`;
   - в `Reset()`: в конце добавить `OnProgressChanged?.Invoke();`.

## Ограничения
- `using System;` уже есть в обоих файлах; новых полей/`using` не добавлять.
- Логику `AddXp`/`Reset`/`TakeDamage` не менять кроме вставки вызовов событий.

## Unity (человеку)
- Не требуется (события — только код; новые сериализуемые поля не вводятся).

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode + `Restart Round`: в `ProgressionSystem` (инспектор) `xp` = 0, `level` = 1 (событие `OnProgressChanged` срабатывает при сбросе).
- Отображение HP/XP через эти события проверяется в T050 (HUDController).

## Отчёт Исполнителя
Готово:
Блокер:
