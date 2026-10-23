# Phase 10.7: Игрок — камера + аркадное движение
- Status: in-progress
- Цель: изометрическая камера, плавно следующая за игроком, и танковое аркадное управление (W/S вдоль «носа», A/D — поворот корпуса) со статами `speed`/`turn` из данных. Без физики, инерции и столкновений (это Фаза 20).

## Задачи (порядок = приоритет)
- T084: `StatBlock` — добавить поле `turn` (источник `ChassisData.turn`) + присвоение в `Init`.
- T085: `PlayerController` — аркадное движение: W/S вдоль `transform.forward`, A/D — yaw-поворот; чтение `statBlock.moveSpeed`/`statBlock.turn`.
- T086: `CameraFollow` — новый скрипт камеры: следует за игроком с фиксированным изометрическим поворотом и сглаживанием позиции.

## Setup for smoke test (human)
1. T086: на `Main Camera` добавить `CameraFollow`, `Target` → `Player`, rotation ≈ (45, 45, 0).
2. Убедиться, что на `Player` заполнены `PlayerController.statBlock` → `StatBlock` (на `Player`) и `PlayerController.moveAction` → action `Move` из `Assets/InputSystem_Actions.inputactions` (стоят по T007).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Камера плавно следует за игроком; угол изометрический и не вращается при движении.
- [ ] W/S двигают танк вперёд/назад по направлению «носа» (`transform.forward`), A/D поворачивают корпус вокруг вертикальной оси.
- [ ] Смена `SessionState.SelectedChassis` (Standart↔Fat) меняет скорость движения (`speed`) и скорость поворота (`turn`) без правки кода.
- [ ] Dev Menu (F1) показывает `turn` в группе `ChassisData…` (рефлексия подхватила поле `ChassisData.turn`); `Player` → `StatBlock` → `Turn` равно `ChassisData.turn`.
