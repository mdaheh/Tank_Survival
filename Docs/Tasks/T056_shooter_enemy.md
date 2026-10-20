# T056: Shooter — data-only (SO + префаб + состав)
- Status: [x]
- Phase: 8
- Depends: T054
- Files:
  - (нет — задача data-only, правок `.cs` нет)

## Задача
Добавить врага Shooter (Стрелок) как данные: `EnemyData`-ассет со статами
из `Docs/GDD/Enemies/Shooter.md` (behavior `ranged`), префаб-дубликат
`Runner.prefab` с заполненным `projectilePrefab`, и третий состав в
`WaveData` (добавить к существующим Runner/Tank).

## Ограничения
- Код не меняется (ranged-ветка `EnemyController` готова в T054).
- Статы Shooter из GDD: HP 20, Speed 6.0, Damage 10, XP 4, behavior
  `ranged`.
- `shootRange`/`shootInterval` GDD не регламентирует — задаёт человек
  (баланс); рекомендация `shootRange` = 8, `shootInterval` = 1.5.

## Unity (человеку)
1. Project → `Assets/Data/Enemies` → правый клик → Create → Tank Survival →
   Enemy Data. Назвать `Shooter`. Инспектор: `health` = 20, `speed` = 6.0,
   `damage` = 10, `xpReward` = 4, `behavior` = `ranged`, `shootRange` = 8,
   `shootInterval` = 1.5.
2. Project → `Assets/Prefabs/Enemies` → Duplicate `Runner.prefab` →
   `Shooter`. В компоненте `EnemyController` заполнить `projectilePrefab` →
   `Assets/Prefabs/Enemies/EnemyProjectile.prefab`.
3. Project → `Assets/Data/Waves` → `Wave1.asset` → массив `compositions`:
   добавить элемент `enemy` → `Shooter.asset`, `prefab` → `Shooter.prefab`,
   `baseCount`/`increase` — баланс (рекомендация `baseCount` = 1,
   `increase` = 1).

## Проверка человеком
- `Shooter.asset`: 20 / 6.0 / 10 / 4, `behavior` = `ranged`.
- `Shooter.prefab`: компоненты `EnemyController` + `EnemyHealth`,
  `projectilePrefab` заполнен.
- `Wave1.asset` → `compositions` содержит Runner, Tank и Shooter.
- Play Mode: Shooter спавнится, идёт к игроку, стреляет на дистанции
  `shootRange`, снаряд наносит урон игроку; умирает от урона.

## Отчёт Исполнителя
Готово: правок `.cs` не требуется — задача data-only (ranged-ветка, снаряд, prewarm и поля EnemyData готовы в T052–T055).
Блокер: —
