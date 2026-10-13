# T002: StatBlock — единый контейнер статов игрока
- Status: [x]
- Phase: 1
- Priority: Critical
- Depends: T001
- Files:
  - Assets/Scripts/Core/StatBlock.cs
  - Assets/Scripts/Data/ChassisData.cs

## Задача
Создать класс `StatBlock : MonoBehaviour` с полями `float damage`, `float fireRate`, `float moveSpeed`, `float maxHealth` и методом `Init(ChassisData chassis)`, который задаёт `moveSpeed = chassis.speed` и `maxHealth = chassis.health`. Поля `damage` и `fireRate` в этой фазе не заполняются (заполнятся из TurretData в Phase 2).

## Ограничения
- Только хранение и `Init`; без расчётов ДПС и прочей логики.
- В `Init` не трогать `damage` и `fireRate`.

## Unity (человеку)
- Не требуется (компонент размещается на Player в T007).

## Проверка человеком
- Компиляция без ошибок; после сборки сцены в T007 `moveSpeed`/`maxHealth` соответствуют выбранному ChassisData.

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/StatBlock.cs — создан класс StatBlock : MonoBehaviour с полями damage, fireRate, moveSpeed, maxHealth и методом Init(ChassisData), задающим moveSpeed = chassis.speed, maxHealth = chassis.health (damage/fireRate не трогаются); Assets/Scripts/Data/ChassisData.cs — без изменений, использован как есть.
Блокер: —
