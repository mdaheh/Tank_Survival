# T020: Множители сложности в EnemyHealth и EnemyController
- Status: [x]
- Phase: 3
- Files:
  - Assets/Scripts/Enemy/EnemyHealth.cs
  - Assets/Scripts/Enemy/EnemyController.cs

## Задача
Добавить публичные поля-множители и применить их.
В `EnemyHealth`: добавить `public float healthMultiplier = 1f`; в `Init(EnemyData data)`
присваивать `currentHealth = data.health * healthMultiplier`; в `OnGetFromPool()`
присваивать `currentHealth = data.health * healthMultiplier`.
В `EnemyController`: добавить `public float speedMultiplier = 1f`; в `Update` двигать врага
к цели со скоростью `data.speed * speedMultiplier`.

## Ограничения
- Не менять сигнатуры `Init` и порядок существующих вызовов.
- Источник значений: `DifficultyData.healthMultiplier` / `DifficultyData.speedMultiplier`,
  присваиваются в `WaveSpawner` (T021). Сейчас поля остаются со значением 1 — временный
  спавн в `GameBootstrap` продолжает работать без изменений.
- Без аллокаций в `Update`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- В инспекторе компонентов `EnemyHealth` и `EnemyController` видны поля
  `healthMultiplier` / `speedMultiplier` со значением 1.

## Отчёт Исполнителя
Готово: Assets/Scripts/Enemy/EnemyHealth.cs: поле healthMultiplier и множитель в Init/OnGetFromPool, Assets/Scripts/Enemy/EnemyController.cs: поле speedMultiplier и скорость data.speed * speedMultiplier в Update
Блокер: —
