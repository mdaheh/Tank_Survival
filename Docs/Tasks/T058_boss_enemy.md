# T058: Boss — data-only (SO + префаб + состав)
- Status: [x]
- Phase: 8
- Files:
  - (нет — задача data-only, правок `.cs` нет)

## Задача
Добавить врага Boss (Босс) как данные: `EnemyData`-ассет со статами из
`Docs/GDD/Enemies/Boss.md`, префаб-дубликат `Runner.prefab` и состав в
`WaveData`. Тег `boss` в этой фазе не вводится — он понадобится только в
Phase 16 (полная система волн, босс на 10-й волне).

## Ограничения
- Код не меняется (Boss — обычный melee-враг с большими статами).
- Статы Boss из GDD: HP 160, Speed 3.0, Damage 40, XP 50, behavior `melee`.

## Unity (человеку)
1. Project → `Assets/Data/Enemies` → Create → Tank Survival → Enemy Data.
   Назвать `Boss`. Инспектор: `health` = 160, `speed` = 3.0, `damage` = 40,
   `xpReward` = 50, `behavior` = `melee`.
2. Project → `Assets/Prefabs/Enemies` → Duplicate `Runner.prefab` → `Boss`
   (компоненты `EnemyController` + `EnemyHealth`; `projectilePrefab` пустой).
3. Project → `Assets/Data/Waves` → `Wave1.asset` → `compositions`: добавить
   элемент `enemy` → `Boss.asset`, `prefab` → `Boss.prefab`, `baseCount` = 1,
   `increase` = 0 (для проверки в текущей упрощённой системе волн; в Phase 16
   Boss перейдёт на спавн каждую 10-ю волну).

## Проверка человеком
- `Boss.asset`: 160 / 3.0 / 40 / 50.
- `Boss.prefab` с `EnemyController` + `EnemyHealth`.
- `Wave1.asset` → `compositions` содержит Boss.
- Play Mode: Boss спавнится, идёт к игроку, бьёт melee, умирает от урона.

## Отчёт Исполнителя
Готово: правок `.cs` не требуется — задача data-only (Boss — melee-враг, melee-пайплайн уже готов; тег boss отложен в Phase 16).
Блокер: —
