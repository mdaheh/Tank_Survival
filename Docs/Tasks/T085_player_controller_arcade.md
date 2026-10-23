# T085: PlayerController — аркадное танковое движение
- Status: [x]
- Phase: 10.7
- Depends: T084
- Files:
  - Assets/Scripts/Player/PlayerController.cs
  - Assets/Scripts/Core/StatBlock.cs

## Задача
В `Assets/Scripts/Player/PlayerController.cs` (namespace `TankSurvival.Player`) заменить тело метода `Update` на аркадное танковое управление. Прочитать `Vector2 input = moveAction.action.ReadValue<Vector2>();` (как сейчас), затем:
1. Поворот корпуса вокруг локальной вертикальной оси: `transform.Rotate(0f, input.x * statBlock.turn * Time.deltaTime, 0f, Space.Self);` — `input.x` (A/D) задаёт yaw, скорость из `statBlock.turn`.
2. Движение вдоль направления «носа» (локальный forward): `transform.Translate(Vector3.forward * (input.y * statBlock.moveSpeed * Time.deltaTime), Space.Self);` — `input.y` (W/S) задаёт вперёд/назад, скорость из `statBlock.moveSpeed`.

Удалить старую строку `transform.Translate(...)` с глобальными осями (`Space.World`). Поля `statBlock` и `moveAction` и `using`-блоки не менять.

## Ограничения
- Менять только `Assets/Scripts/Player/PlayerController.cs` (тело `Update`).
- Ноль аллокаций в `Update`: использовать `Vector3.forward` и скалярные произведения, не создавать `new Vector3`/строки в кадре.
- Не добавлять Rigidbody, физику, инерцию и столкновения (Фаза 20).
- Направление поворота: `input.x * turn` даёт A = влево, D = вправо при взгляде сверху. Если на тесте направление окажется инвертированным (зависит от ориентации «носа» `ChassisVisual` — это просто Cube без маркировки носа), это единственная точка подстройки: изменить знак слагаемого поворота на `-input.x`.

## Unity (человеку)
- Проверить на объекте `Player`: `PlayerController.statBlock` → компонент `StatBlock` (на `Player`), `PlayerController.moveAction` → action `Move` (map `Player`) из `Assets/InputSystem_Actions.inputactions`. Ссылки уже стоят по T007; менять только при необходимости.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode: W/S двигают танк вперёд/назад по направлению «носа» (вдоль длинной оси `ChassisVisual`), A/D поворачивают корпус вокруг вертикальной оси; W+A даёт дугу.
- Смена `SessionState.SelectedChassis` Standart↔Fat меняет скорость движения и скорость поворота без правки кода.

## Отчёт Исполнителя
Готово: —
Блокер: T085: блокер — жалоба «поворот инвертирован только при движении назад (S + A/D)» не устраняется единственной точкой подстройки из ТЗ: смена знака на `-input.x` инвертирует поворот и при движении вперёд. Вопрос: нужен ли знак поворота от направления движения (`input.x * Mathf.Sign(input.y) * statBlock.turn`) и что делать при `input.y == 0`?

## Отчёт Человека
Закрыл задачу человек, поменяв строчку transform.Rotate(0f, input.x * statBlock.turn * Time.deltaTime, 0f, Space.Self);
на строчку transform.Rotate(0f, input.x * Mathf.Sign(input.y) * statBlock.turn * Time.deltaTime, 0f, Space.Self);
Это помогло быстро решить проблему инвентированного движения назад.