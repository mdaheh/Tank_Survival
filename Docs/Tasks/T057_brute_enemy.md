# T057: Brute — data-only (SO + префаб + состав)
- Status: [x]
- Phase: 8
- Files:
  - (нет — задача data-only, правок `.cs` нет)

## Задача
Добавить врага Brute (Грубиян) как данные: `EnemyData`-ассет со статами из
`Docs/GDD/Enemies/Brute.md`, префаб-дубликат `Runner.prefab` и состав в
`WaveData`.

## Ограничения
- Код не меняется (melee-пайплайн уже поддерживает произвольного врага).
- Статы Brute из GDD: HP 40, Speed 6.0, Damage 10, XP 3, behavior `melee`.

## Unity (человеку)
1. Project → `Assets/Data/Enemies` → Create → Tank Survival → Enemy Data.
   Назвать `Brute`. Инспектор: `health` = 40, `speed` = 6.0, `damage` = 10,
   `xpReward` = 3, `behavior` = `melee`.
2. Project → `Assets/Prefabs/Enemies` → Duplicate `Runner.prefab` → `Brute`
   (компоненты `EnemyController` + `EnemyHealth`; `projectilePrefab` пустой).
3. Project → `Assets/Data/Waves` → `Wave1.asset` → `compositions`: добавить
   элемент `enemy` → `Brute.asset`, `prefab` → `Brute.prefab`,
   `baseCount`/`increase` — баланс (рекомендация `baseCount` = 1,
   `increase` = 1).

## Проверка человеком
- `Brute.asset`: 40 / 6.0 / 10 / 3.
- `Brute.prefab` с `EnemyController` + `EnemyHealth`.
- `Wave1.asset` → `compositions` содержит Brute.
- Play Mode: Brute спавнится, идёт к игроку, бьёт melee, умирает от урона.

## Отчёт Исполнителя
Готово: правок `.cs` не требуется — задача data-only (melee-пайплайн поддерживает произвольного врага, код в T052–T056 не меняется).
Блокер: —
