# T008: Namespace `TankSurvival.*` для существующих файлов
- Status: [x]
- Phase: 2
- Priority: High
- Files:
  - Assets/Scripts/Data/ChassisData.cs
  - Assets/Scripts/Core/GameBootstrap.cs
  - Assets/Scripts/Core/IDamageable.cs
  - Assets/Scripts/Core/SessionState.cs
  - Assets/Scripts/Core/StatBlock.cs
  - Assets/Scripts/Player/PlayerController.cs
  - Assets/Scripts/Player/PlayerHealth.cs

## Задача
Обернуть каждый класс в namespace по папке и добавить `using` для кросс-ссылок:
`ChassisData` → `TankSurvival.Data`; `GameBootstrap`, `IDamageable`, `SessionState`,
`StatBlock` → `TankSurvival.Core`; `PlayerController`, `PlayerHealth` → `TankSurvival.Player`.
`SessionState` и `StatBlock` добавляют `using TankSurvival.Data;`; `PlayerController` и
`PlayerHealth` — `using TankSurvival.Core;`; `GameBootstrap` — `using TankSurvival.Player;`.

## Ограничения
- Имена классов, полей, методов и сигнатуры не менять.
- Логику не трогать; только namespace-обёртка и `using`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- В сцене всё работает как в фазе 1 (танк движется, HP инициализируется).

## Отчёт Исполнителя
Готово: ChassisData.cs: namespace TankSurvival.Data; GameBootstrap.cs: namespace TankSurvival.Core + using TankSurvival.Player; IDamageable.cs: namespace TankSurvival.Core; SessionState.cs: namespace TankSurvival.Core + using TankSurvival.Data; StatBlock.cs: namespace TankSurvival.Core + using TankSurvival.Data; PlayerController.cs: namespace TankSurvival.Player + using TankSurvival.Core; PlayerHealth.cs: namespace TankSurvival.Player + using TankSurvival.Core
Блокер: —
