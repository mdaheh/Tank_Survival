# T005: PlayerController — движение игрока
- Status: [x]
- Phase: 1
- Priority: High
- Depends: T002
- Files:
  - Assets/Scripts/Player/PlayerController.cs
  - Assets/Scripts/Core/StatBlock.cs

## Задача
Создать класс `PlayerController : MonoBehaviour` с сериализованными полями `StatBlock statBlock` и `InputActionReference moveAction`. В `Update` читать `moveAction.ReadValue<Vector2>()` и перемещать объект по горизонтальной плоскости со скоростью `statBlock.moveSpeed` (units/sec) в направлении ввода (x→мировая X, y→мировая Z).

## Ограничения
- Ноль аллокаций в `Update` (не создавать векторы/строки в кадре).
- Не трогать HP и другие системы.

## Unity (человеку)
- Создать Input Action asset `Assets/Settings/Controls.inputactions`: action map "Player", action "Move" (Value, Vector2), привязки WASD + стрелки.
- Размещение компонента на Player и привязка `moveAction` — в T007.

## Проверка человеком
- Компиляция без ошибок; после сборки сцены в T007 танк движется по WASD со скоростью из ChassisData.

## Отчёт Исполнителя
Готово: Assets/Scripts/Player/PlayerController.cs — создан класс PlayerController : MonoBehaviour с сериализованными полями statBlock и moveAction; в Update чтение moveAction.action.ReadValue<Vector2>() и перемещение по X/Z со скоростью statBlock.moveSpeed (Space.World, без аллокаций); Assets/Scripts/Core/StatBlock.cs — без изменений, использован как есть.
Блокер: —
