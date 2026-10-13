# T006: PlayerHealth — HP игрока
- Status: [x]
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
Готово: Assets/Scripts/Player/PlayerHealth.cs — создан класс PlayerHealth : MonoBehaviour, IDamageable со сериализованным statBlock, полем currentHealth (доступ на чтение через CurrentHealth), событием OnDeath (System.Action) и Init() (currentHealth = statBlock.maxHealth); TakeDamage уменьшает currentHealth и при <= 0 вызывает OnDeath; Assets/Scripts/Core/StatBlock.cs и Assets/Scripts/Core/IDamageable.cs — без изменений, использованы как есть.
Блокер: —
