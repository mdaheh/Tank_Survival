# T004: SessionState — держатель выбора
- Status: [x]
- Phase: 1
- Priority: High
- Depends: T001
- Files:
  - Assets/Scripts/Core/SessionState.cs
  - Assets/Scripts/Data/ChassisData.cs

## Задача
Создать класс `SessionState : MonoBehaviour` с сериализованным полем `ChassisData selectedChassis` и публичным доступом на чтение `SelectedChassis`. В этой фазе — только шасси; поля турели/сложности добавляются аддитивно в Phase 3/7.

## Ограничения
- Только хранение выбора; без логики старта/сброса раунда.

## Unity (человеку)
- Не требуется (компонент размещается на Bootstrap в T007).

## Проверка человеком
- Компиляция без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/SessionState.cs — создан класс SessionState : MonoBehaviour с сериализованным полем ChassisData selectedChassis и доступом на чтение SelectedChassis (только хранение выбора); Assets/Scripts/Data/ChassisData.cs — без изменений, использован как есть.
Блокер: —
