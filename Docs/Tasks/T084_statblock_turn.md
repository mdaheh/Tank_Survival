# T084: StatBlock — поле turn
- Status: [ ]
- Phase: 10.7
- Files:
  - Assets/Scripts/Core/StatBlock.cs

## Задача
В `Assets/Scripts/Core/StatBlock.cs` (namespace `TankSurvival.Core`) добавить публичное поле `public float turn;` рядом с существующими `damage`/`fireRate`/`moveSpeed`/`maxHealth`. В методе `Init(ChassisData chassis, TurretData turret)` добавить присвоение `turn = chassis.turn;` сразу после `moveSpeed = chassis.speed;`. Источник значения — `ChassisData.turn` (SO, namespace `TankSurvival.Data`); потребитель — `PlayerController` (T085). Апгрейда для `turn` нет — `UpgradeData.StatType` не расширять.

## Ограничения
- Менять только `Assets/Scripts/Core/StatBlock.cs`.
- Сигнатуру `Init(ChassisData chassis, TurretData turret)` и существующие присвоения (`moveSpeed`, `maxHealth`, `damage`, `fireRate`) не менять.
- `ApplyUpgrade`, событие `MaxHealthIncreased` и `UpgradeData` не трогать.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode после старта раунда: объект `Player` → компонент `StatBlock` → поле `Turn` равно `ChassisData.turn` выбранного шасси.
- Dev Menu (F1) → группа `ChassisData…` → слайдер `turn` присутствует и равен значению ассета (рефлексия `DevParamRegistry` подхватывает `public float` поля SO).

## Отчёт Исполнителя
Готово:
Блокер:
