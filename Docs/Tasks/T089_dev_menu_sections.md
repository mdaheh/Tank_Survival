# T089: Dev Menu — секции/категории
- Status: [x]
- Phase: 10.8
- Files:
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Сгруппировать параметры Dev Menu в секции по категориям (Chassis / Turret / Delivery / OnHit / Difficulty / Enemy) вместо заголовков по полному пути группы.

`Assets/Scripts/Dev/DevMenuController.cs`:
1. Добавить приватный статический метод категоризации:
   ```csharp
   private static string GetCategory(string group)
   {
       if (group.StartsWith("ChassisData")) return "Chassis";
       if (group.EndsWith(".delivery")) return "Delivery";
       if (group.StartsWith("TurretData")) return "Turret";
       if (group.StartsWith("OnHitModuleData")) return "OnHit";
       if (group.StartsWith("DifficultyData")) return "Difficulty";
       if (group.StartsWith("EnemyData")) return "Enemy";
       return group;
   }
   ```
   (порядок проверок важен: `.delivery` проверяется до `TurretData`).
2. В `BuildRows()`: вместо отслеживания `currentGroup` по `parameter.Group` отслеживать `string currentCategory = null;` и вычислять `string category = GetCategory(parameter.Group);`. Когда `category != currentCategory` — присвоить `currentCategory = category;` и вызвать `CreateSectionHeader(content, category);`.
3. Заменить метод `CreateGroupHeader(RectTransform content, string group)` на `CreateSectionHeader(RectTransform content, string category)`:
   - блок-заголовок секции той же высоты `GroupHeaderHeight`, но с фоновым `Image` цвета `new Color(0.14f, 0.20f, 0.28f, 1f)` (визуально отделяет секцию от строк);
   - внутри — текст названия категории: `fontStyle = FontStyle.Bold`, цвет `new Color(0.75f, 0.9f, 1f, 1f)`, прижат слева (`Stretch(label.rectTransform, new Vector2(8f, 0f), new Vector2(-8f, 0f))`).

## Ограничения
- Порядок строк параметров не менять (порядок = порядок `DevParamRegistry.All`); меняется только способ группировки/заголовков.
- Категория определяется только строкой `DevParam.Group` (не именем поля и не типом SO).
- Не менять слайдеры, ручной ввод (T090) и скролл (T091).
- Секция «Turret» может отсутствовать (у `TurretData` нет публичных `float`/`int` полей) — это ожидаемо.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → Dev Menu: строки сгруппированы в секции с фоновыми заголовками:
  - «Chassis» — `health`, `speed`, `turn`;
  - «Delivery» — `speed`, `pelletCount`, `spreadAngle`, `range`, `aps`, `damage`, `maxTargets`, `tickRate`, `angle`;
  - «OnHit» — `priority`, `radius`, `damage`, `duration`, `tickDamage`, `tickRate`, `slowAmount`, `freezeThreshold`, `force`, `damageScaling`, `maxDepth`;
  - «Difficulty» — `waveCount`, `countMultiplier`, `healthMultiplier`, `speedMultiplier`, `xpMultiplier`;
  - «Enemy» — `health`, `speed`, `damage`, `xpReward` (и `shootRange`/`shootInterval`).
- Заголовки секций визуально отделены от строк (фоновый блок).

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DevMenuController.cs: добавлен `GetCategory(string group)`; `BuildRows()` переведён на `currentCategory`/`GetCategory(parameter.Group)`; `CreateGroupHeader` заменён на `CreateSectionHeader` (фон `Image` `new Color(0.14f, 0.20f, 0.28f, 1f)`, текст `FontStyle.Bold` цвета `new Color(0.75f, 0.9f, 1f, 1f)`, `Stretch(..., new Vector2(8f, 0f), new Vector2(-8f, 0f))`).
Блокер: —
