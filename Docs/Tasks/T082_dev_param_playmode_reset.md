# T082: DevParam — снапшот с диска + автооткат при выходе из Play Mode
- Status: [!]
- Phase: 10.6
- Files:
  - Assets/Scripts/Dev/DevParam.cs
  - Assets/Scripts/Dev/DevParamRegistry.cs

## Задача
В `DevParam` добавить снапшот исходного значения с диска и метод отката; в `DevParamRegistry` добавить editor-хук, который при выходе из Play Mode восстанавливает все параметры. Итог: правка в Dev Menu не переживает выход из Play Mode при отключённом Domain/Scene Reload.

`Assets/Scripts/Dev/DevParam.cs`:
1. Добавить поле `private readonly float _originalValue;`.
2. В конструкторе, после расчёта `Min`/`Max`, присвоить `_originalValue = ReadOriginalValue();`.
3. Добавить приватный метод `float ReadOriginalValue()`:
   - `#if UNITY_EDITOR`: если `_target is UnityEngine.Object`, создать
     `SerializedObject so = new SerializedObject((UnityEngine.Object)_target);`,
     `SerializedProperty prop = so.FindProperty(_field.Name);`; если `prop != null`, вернуть
     `prop.propertyType == SerializedPropertyType.Integer ? prop.intValue : prop.floatValue`
     (затем `so.Dispose()`); иначе fallback `Convert.ToSingle(_field.GetValue(_target))`.
   - `#else` (DEBUG без редактора): `return Convert.ToSingle(_field.GetValue(_target));`.
4. Добавить `public void Restore() { SetValue(_originalValue); }`.
5. `using UnityEditor;` — обернуть в `#if UNITY_EDITOR` / `#endif`.
6. Существующие `GetValue()`/`SetValue()`/`Min`/`Max` не менять.

`Assets/Scripts/Dev/DevParamRegistry.cs`:
1. В блок using (внутри файлового `#if UNITY_EDITOR || DEBUG`) добавить
   `#if UNITY_EDITOR` / `using UnityEditor;` / `#endif`.
2. В конец класса `DevParamRegistry` добавить (внутри `#if UNITY_EDITOR`):
   - `private static bool _playModeHookInstalled;`
   - `[InitializeOnLoadMethod] private static void InstallPlayModeHook()`:
     если `_playModeHookInstalled` — `return`; иначе `_playModeHookInstalled = true;` и
     `EditorApplication.playModeStateChanged += OnPlayModeStateChanged;`.
   - `private static void OnPlayModeStateChanged(PlayModeStateChange state)`:
     если `state == PlayModeStateChange.ExitingPlayMode`, пройти
     `for (int i = 0; i < All.Count; i++) All[i].Restore();`.

## Ограничения
- Не менять `ProjectSettings/`, `Packages/`, `manifest.json` — Domain/Scene Reload остаются выключенными.
- Вся новая логика снапшота/отката — только в `#if UNITY_EDITOR`; в релиз не компилируется (файлы уже в `#if UNITY_EDITOR || DEBUG`).
- Ноль аллокаций в игровом цикле: `SerializedObject` создаётся один раз в конструкторе при `Init()`; откат — один раз по событию `playModeStateChanged`; `GetValue`/`SetValue` остаются чистой рефлексией без `SerializedObject`/`AssetDatabase`.
- Не трогать `.asset` на диске: откат — через `SetValue` (managed-поле), без `SerializedObject.ApplyModifiedProperties`/`EditorUtility.SetDirty`/`AssetDatabase`.
- Не менять игровые системы (`StatBlock`, `WaveSpawner`, `EnemyHealth`, `TurretController`) — вне задачи.

## Unity (человеку)
Не требуется. Enter Play Mode Options уже настроены («Do not reload Domain» и «Do not reload Scene»); менять их не нужно.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- В Edit Mode открыть ассет ChassisData (Project) — запомнить `Speed`.
- Play Mode → F1 → в Dev Menu слайдер `speed` (группа `ChassisData…`) показывает это значение; изменить его (например, в 2 раза) — текст значения у слайдера меняется.
- Stop → снова Play Mode → F1 → слайдер `speed` снова показывает исходное значение (не правку).
- Инспектор ассета ChassisData в Edit Mode после цикла показывает исходное `Speed` (диск не изменён).
- Повторить «правка → Stop → вход» для `EnemyData` (health): после перезахода слайдер снова равен диску.

## Отчёт Исполнителя
Готово: DevParam.cs: поле `_originalValue` + `ReadOriginalValue()` (SerializedObject под `#if UNITY_EDITOR`, fallback Convert.ToSingle) + `Restore()`; DevParamRegistry.cs: `using UnityEditor` под `#if UNITY_EDITOR`, `InstallPlayModeHook()` с `[InitializeOnLoadMethod]` и `OnPlayModeStateChanged` → `Restore()` всех параметров на `ExitingPlayMode`.
Блокер: CS0104 — после добавления `using UnityEditor;` в DevParamRegistry.cs строка 22 (`SessionState`) стала неоднозначной (TankSurvival.Core.SessionState vs UnityEditor.SessionState). Исправляется в T083.
