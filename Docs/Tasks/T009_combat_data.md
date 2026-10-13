# T009: SO-данные боя — EnemyData, TurretData, DeliveryModuleData
- Status: [ ]
- Phase: 2
- Priority: Critical
- Files:
  - Assets/Scripts/Data/EnemyData.cs
  - Assets/Scripts/Data/TurretData.cs
  - Assets/Scripts/Data/DeliveryModuleData.cs

## Задача
Создать три ScriptableObject-класса в namespace `TankSurvival.Data`.
`EnemyData` (`[CreateAssetMenu(menuName = "Tank Survival/Enemy Data")]`): поля
`float health`, `float speed`, `float damage`, `int xpReward`, `string behavior = "melee"`.
`DeliveryModuleData` (`[CreateAssetMenu(menuName = "Tank Survival/Delivery Module Data")]`):
`enum DeliveryKind { Projectile, Hitscan }` и поля `DeliveryKind kind`, `float speed`,
`int pelletCount`, `float spreadAngle`.
`TurretData` (`[CreateAssetMenu(menuName = "Tank Survival/Turret Data")]`): поля
`float aps`, `float range`, `float damage`, `DeliveryModuleData delivery`.

## Ограничения
- Все три класса в namespace `TankSurvival.Data`.
- Поля публичные; дефолтные числовые значения не задавать (числа кладёт человек в
  `.asset`), кроме `behavior = "melee"`.
- OnHit-модули и список on_hit в `TurretData` не добавлять (это фаза 5).

## Unity (человеку)
Создать ассеты (ПКМ в папке → Create → Tank Survival → нужный тип):
- `Assets/Data/Enemies/Runner.asset` (EnemyData): health 20, speed 12, damage 5, xpReward 1, behavior "melee".
- `Assets/Data/Delivery/Projectile.asset` (DeliveryModuleData): kind Projectile, speed 15, pelletCount 1, spreadAngle 0.
- `Assets/Data/Turrets/Hammer.asset` (TurretData): aps 2, range 6, damage 30, delivery → `Projectile.asset`.

## Проверка человеком
- Каждый ассет в инспекторе показывает перечисленные поля с введёнными значениями.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
