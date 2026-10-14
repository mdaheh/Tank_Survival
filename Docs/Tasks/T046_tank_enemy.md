# T046: Враг Tank (data-only)
- Status: [ ]
- Phase: 7
- Files:
  - (нет — задача data-only, правок `.cs` нет)

## Задача
Добавить второго врага Tank (Защитник) как данные, без изменения кода: создать `EnemyData`-ассет со статами из `Docs/GDD/Enemies/Tank.md`, префаб-дубликат `Runner.prefab` и второй состав в `WaveData`. Оба врага идут по одному melee-пайплайну (`EnemyController`/`EnemyHealth`).

## Ограничения
- Код не меняется (`EnemyData`/`EnemyController`/`EnemyHealth` уже поддерживают произвольный melee-враг через данные).
- Статы Tank брать из GDD: HP 80, Speed 3.0, Damage 5, XP 5, behavior `melee` (значение по умолчанию).

## Unity (человеку)
1. Project → `Assets/Data/Enemies` → правый клик → Create → Tank Survival → Enemy Data. Назвать `Tank`.
   Инспектор `Tank.asset`: `health` = 80, `speed` = 3.0, `damage` = 5, `xpReward` = 5, `behavior` оставить `melee`.
2. Project → `Assets/Prefabs/Enemies` → правый клик по `Runner.prefab` → Duplicate → переименовать в `Tank`.
   Проверить, что на корне префаба есть компоненты `EnemyController` и `EnemyHealth` (как у `Runner`). Внутренние ссылки (`health`, `data`) заполнять не нужно — они проставляются при спавне через `controller.Init`.
3. Project → `Assets/Data/Waves` → выбрать `Wave1.asset` → в инспекторе у массива `compositions` добавить второй элемент:
   - `enemy` → `Assets/Data/Enemies/Tank.asset`;
   - `prefab` → `Assets/Prefabs/Enemies/Tank.prefab`;
   - `baseCount` и `increase` — значения задаёт человек (баланс); рекомендация `baseCount` = 1, `increase` = 1, чтобы Tank появлялся в волнах.

## Проверка человеком
- `Tank.asset` в `Assets/Data/Enemies` с полями 80 / 3.0 / 5 / 5.
- `Tank.prefab` в `Assets/Prefabs/Enemies` с компонентами `EnemyController` + `EnemyHealth`.
- `Wave1.asset` → `compositions` содержит 2 элемента (Runner и Tank).
- Play Mode: в волне спавнятся и Runner, и Tank; оба идут к игроку и умирают от урона.

## Отчёт Исполнителя
Готово:
Блокер:
