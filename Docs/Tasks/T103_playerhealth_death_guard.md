# T103: PlayerHealth.TakeDamage — guard после смерти
- Status: [x]
- Phase: 10.10
- Files:
  - Assets/Scripts/Player/PlayerHealth.cs

## Задача
В методе `TakeDamage(float amount)` класса `PlayerHealth` добавить guard от повторного урона после смерти: если `currentHealth <= 0f` — выйти, не уменьшая здоровье и не вызывая `OnDeath` повторно (зеркально `EnemyHealth.TakeDamage`).

В `Assets/Scripts/Player/PlayerHealth.cs`, в начало тела `TakeDamage` добавить:
```csharp
if (currentHealth <= 0f)
{
    return;
}
```

## Ограничения
- После `Init()` (StartRound) `currentHealth = statBlock.maxHealth`, поэтому guard не блокирует первый урон.
- `OnHealthChanged`/`OnDeath` больше не вызываются повторно после смерти — `RoundController.HandlePlayerDeath` срабатывает ровно один раз.
- Поведение идентично существующему guard в `EnemyHealth.TakeDamage`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Start Round → довести игрока до смерти при одновременных ударах нескольких врагов. Убедиться: экран Lose появляется один раз, HP не уходит ниже нуля повторными ударами, повторного срабатывания Lose нет.

## Отчёт Исполнителя
Готово: Assets/Scripts/Player/PlayerHealth.cs: в начало тела `TakeDamage` добавлен guard `if (currentHealth <= 0f) { return; }` — повторный урон после смерти не уменьшает здоровье и не вызывает `OnHealthChanged`/`OnDeath` повторно.
Блокер: —
