# T077: DevParamRegistry — авто-регистрация параметров
- Status: [x]
- Phase: 10.5
- Files:
  - Assets/Scripts/Dev/DevParam.cs
  - Assets/Scripts/Dev/DevParamRegistry.cs

## Задача
Создать dev-слой `TankSurvival.Dev` (оба файла обёрнуты в `#if UNITY_EDITOR || DEVELOPMENT_BUILD`).
`DevParam` — модель параметра: `string Group`, `string Label`, `bool IsInteger`, `float Min`, `float Max`,
`float GetValue()`, `void SetValue(float)`. Get/Set через кэшированный `FieldInfo` целевого объекта
(`GetValue` = `Convert.ToSingle(field.GetValue(target))`; `SetValue` для `int` округляет
`Mathf.RoundToInt(value)` перед `field.SetValue`).

`DevParamRegistry.Reflect(object target, string group)` — рефлексия `public` полей `float`/`int` цели
(прочие поля: `GameObject`/`enum`/`string`/массивы/ссылки на SO — пропускаются) → `List<DevParam>`.

`DevParamRegistry.All` — предсобранный список SO-целей (собирается в `Init()`):
- `SessionState.SelectedChassis`, `SelectedTurret` (и его `delivery`, `onHitModules[]`), `SelectedDifficulty`;
- `WaveSpawner.waveData.compositions[]` → каждый `EnemyData`.

Ссылки на `SessionState`/`WaveSpawner` брать через `Object.FindFirstObjectByType<T>()` (один раз в `Init()`).
Метка группы: `EnemyData.<имя>`, `ChassisData.<имя>`, `TurretData.<имя>.delivery`, `OnHitModuleData.<имя>`, `DifficultyData.<имя>`.
Диапазон по умолчанию: `Min = 0`, `Max = max(10, current * 4)` (для `int` — округление); если значение 0 — диапазон `0..10`.

## Ограничения
- Весь код — только в `#if UNITY_EDITOR || DEVELOPMENT_BUILD`; в релизе не компилируется.
- Рефлексия — только при `Init()`/по требованию; `GetValue`/`SetValue` — по событию UI, не в `Update`.
- `SetValue` пишет в инстанс SO/компонента в памяти через `FieldInfo.SetValue` — не трогает `.asset` на диске и не использует `SerializedObject`/`AssetDatabase`.
- Новое поле в зарегистрированном типе появляется само; новый тип SO — одна строка в `Init()`.
- Геометрические поля (`angle`/`range`/`radius` в `DeliveryModuleData`/`OnHitModuleData`) не фильтруются —
  остаются редактируемыми. Синхронизацию визуального маркера с новыми значениями выполняет
  `DevVisualizer` (T081): масштаб трансформа для `range`/`radius`, пересчёт дуги для `angle`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений; в Play Mode реестр инициализируется без исключений (консоль чистая).
- Полная проверка авто-регистрации выполняется в T078 (временное поле `EnemyData` видно в Dev Menu).

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DevParam.cs: создана модель параметра (public readonly Group/Label/IsInteger/Min/Max, GetValue через Convert.ToSingle(field.GetValue(target)), SetValue с Mathf.RoundToInt для int, диапазон Min=0/Max=max(10,current*4) с округлением для int), файл целиком в #if UNITY_EDITOR || DEVELOPMENT_BUILD; Assets/Scripts/Dev/DevParamRegistry.cs: создан реестр (Reflect(object,string) — рефлексия public float/int полей с пропуском прочих, public static All и Init() со сбором SessionState.SelectedChassis/SelectedTurret(+delivery,+onHitModules[])/SelectedDifficulty и WaveSpawner.waveData.compositions[].enemy через Object.FindFirstObjectByType, метки групп ChassisData.<имя>/TurretData.<имя>/TurretData.<имя>.delivery/OnHitModuleData.<имя>/DifficultyData.<имя>/EnemyData.<имя>), файл целиком в #if UNITY_EDITOR || DEVELOPMENT_BUILD.
Блокер: —
