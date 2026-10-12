# Phase 1: Фундамент — данные и живой танк
- Status: in-progress
- Цель: запустить вертикальный срез «живой танк»: игрок управляется по вводу,
  скорость и максимальное HP приходят из `ChassisData` (ScriptableObject) через
  `StatBlock`, а не из констант. `GameBootstrap` поднимает подсистемы.
  Доказать: данные — источник статов, а не декорация.

## Задачи (порядок = приоритет)
- T001: `ChassisData` — SO шасси (health/speed/turn + зарезервированное поле способности).
- T002: `StatBlock` — единый контейнер статов игрока, инициализация из `ChassisData`.
- T003: `IDamageable` — интерфейс урона «это можно повредить».
- T004: `SessionState` — держатель выбранного шасси (переживает рестарт).
- T005: `PlayerController` — движение по Input System, чтение move_speed из `StatBlock`.
- T006: `PlayerHealth` — текущее HP, реализация `IDamageable`, init из `StatBlock.max_health`.
- T007: `GameBootstrap` — composition root: инициализация StatBlock и игрока.

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Сцена запускается; консоль без ошибок.
- [ ] Танк движется по вводу (WASD), скорость соответствует ChassisData Standart (medium).
- [ ] PlayerHealth.current = StatBlock.max_health (значение из ChassisData) — видно в инспекторе.
- [ ] Смена ChassisData (Standart → Fat) в SessionState меняет скорость и max HP без правки кода.
- [ ] Поле способности в ChassisData существует, но нигде не используется (компилируется).
