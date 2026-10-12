# T006: PlayerHealth — HP игрока
- Status: [ ]
- Phase: 1
- Priority: High
- Depends: T002, T003
- Files:
  - Assets/Scripts/Player/PlayerHealth.cs
  - Assets/Scripts/Core/StatBlock.cs
  - Assets/Scripts/Core/IDamageable.cs

## Задача
Создать класс `PlayerHealth : MonoBehaviour, IDamageable` с сериализованным полем `StatBlock statBlock`, полем `float currentHealth` (только чтение снаружи), событием `System.Action OnDeath` и методом `Init()`, задающим `currentHealth = statBlock.maxHealth`. Реализовать `TakeDamage(float amount)`: уменьшить `currentHealth`, при `currentHealth <= 0` вызвать `OnDeath`.

## Ограничения
- Не трогать движение и ввод.
- `Init()` вызывается только из GameBootstrap (T007).

## Unity (человеку)
- Не требуется (компонент размещается на Player в T007).

## Проверка человеком
- Компиляция без ошибок; после T007 `currentHealth` = `maxHealth` из ChassisData.

## Отчёт Исполнителя
Готово:
Блокер:
