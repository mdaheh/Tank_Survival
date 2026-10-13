# T001: ChassisData — ScriptableObject шасси
- Status: [x]
- Phase: 1
- Priority: Critical
- Files:
  - Assets/Scripts/Data/ChassisData.cs

## Задача
Создать класс `ChassisData : ScriptableObject` с `[CreateAssetMenu(menuName = "Tank Survival/Chassis Data", fileName = "ChassisData")]` и сериализованными полями:
`float health`, `float speed`, `float turn`, `string abilityId` (зарезервировано под способность — в MVP пусто и не используется).

## Ограничения
- Только данные, без методов поведения и логики.
- `abilityId` нигде не читается и не пишется в рантайме.

## Unity (человеку)
- Создать 2 ассета через меню Create → Tank Survival → Chassis Data:
  - `Assets/Settings/Chassis/Standart.asset`: health=100, speed=5.0, turn=180.
  - `Assets/Settings/Chassis/Fat.asset`: health=200, speed=2.5, turn=135.
- Значения — из Legend: base chassis_hp=100, chassis_speed=5.0, chassis_turn=180,
  умноженные на шкалу шасси (Standart medium=1.0; Fat very_high=2.0 / very_low=0.5 / low=0.75).

## Проверка человеком
- В Project видны `Standart.asset` и `Fat.asset`; в инспекторе поля заполнены и редактируются; компиляция без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Data/ChassisData.cs — создан класс ChassisData : ScriptableObject с атрибутом CreateAssetMenu и полями health, speed, turn, abilityId.
Блокер: —
